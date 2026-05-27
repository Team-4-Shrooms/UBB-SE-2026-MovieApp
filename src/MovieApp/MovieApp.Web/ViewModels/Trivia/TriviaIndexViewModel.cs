using MovieApp.DataLayer.Models;

namespace MovieApp.Web.ViewModels.Trivia
{
    public sealed class TriviaIndexViewModel
    {
        public TriviaQuestion? Question { get; set; }
        public string? Category { get; set; }
        public List<string> AvailableCategories { get; set; } = new();

        /// <summary>null = first visit; true/false = result of last submitted answer.</summary>
        public bool? LastAnswerCorrect { get; set; }

        public char? LastSelectedOption { get; set; }
        public int? RewardId { get; set; }
    }
}
