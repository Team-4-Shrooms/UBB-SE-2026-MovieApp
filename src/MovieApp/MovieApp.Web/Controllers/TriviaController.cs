using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.ViewModels.Trivia;

namespace MovieApp.Web.Controllers
{
    public sealed class TriviaController : Controller
    {
        private readonly ITriviaService _triviaService;
        private readonly ICurrentUserService _currentUserService;

        public TriviaController(ITriviaService triviaService, ICurrentUserService currentUserService)
        {
            _triviaService = triviaService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? category = null)
        {
            List<TriviaQuestion> allQuestions = await _triviaService.GetAllQuestionsAsync();

            List<string> categories = allQuestions
                .Select(question => question.Category)
                .Distinct()
                .OrderBy(categoryName => categoryName)
                .ToList();

            List<TriviaQuestion> pool = string.IsNullOrEmpty(category)
                ? allQuestions
                : allQuestions.Where(question => question.Category == category).ToList();

            TriviaQuestion? question = pool.Count > 0
                ? pool[Random.Shared.Next(pool.Count)]
                : null;

            bool? lastAnswerCorrect = TempData["LastAnswerCorrect"] as bool?;
            char? lastSelectedOption = TempData["LastSelectedOption"] is string tempDataOptionString && tempDataOptionString.Length == 1
                ? tempDataOptionString[0]
                : null;
            int? rewardId = TempData["RewardId"] as int?;

            TriviaIndexViewModel viewModel = new TriviaIndexViewModel
            {
                Question = question,
                Category = category,
                AvailableCategories = categories,
                LastAnswerCorrect = lastAnswerCorrect,
                LastSelectedOption = lastSelectedOption,
                RewardId = rewardId,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(TriviaAnswerInputModel model)
        {
            TriviaQuestion? question = await _triviaService.GetQuestionByIdAsync(model.QuestionId);

            if (question is null)
            {
                return RedirectToAction(nameof(Index), new { category = model.Category });
            }

            bool correct = question.CorrectOption == model.SelectedOption;

            TempData["LastAnswerCorrect"] = correct;
            TempData["LastSelectedOption"] = model.SelectedOption.ToString();

            if (correct)
            {
                int rewardId = await _triviaService.AwardRewardAsync(_currentUserService.UserId);
                TempData["RewardId"] = rewardId;
            }

            return RedirectToAction(nameof(Index), new { category = model.Category });
        }
    }
}
