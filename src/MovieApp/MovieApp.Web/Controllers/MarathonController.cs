using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers;

public sealed class MarathonController : Controller
{
    private readonly IMarathonService _marathonService;
    private readonly ICurrentUserService _currentUserService;

    public MarathonController(IMarathonService marathonService, ICurrentUserService currentUserService)
    {
        _marathonService = marathonService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = _currentUserService.UserId;

        IEnumerable<Marathon> marathons = await _marathonService.GetWeeklyMarathonsAsync(userId);

        List<MarathonListItemViewModel> items = new List<MarathonListItemViewModel>();
        foreach (Marathon marathon in marathons)
        {
            int totalMovies = await _marathonService.GetMarathonMovieCountAsync(marathon.Id);
            int participants = await _marathonService.GetParticipantCountAsync(marathon.Id);
            MarathonProgress? progress = await _marathonService.GetUserProgressAsync(userId, marathon.Id);

            items.Add(new MarathonListItemViewModel
            {
                Marathon = marathon,
                TotalMovies = totalMovies,
                CompletedMovies = progress?.CompletedMoviesCount ?? 0,
                ParticipantCount = participants,
                IsJoined = progress is not null,
            });
        }

        MarathonIndexViewModel viewModel = new MarathonIndexViewModel
        {
            Items = items,
            StatusMessage = TempData["StatusMessage"] as string,
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        int userId = _currentUserService.UserId;

        IEnumerable<Marathon> weeklyMarathons = await _marathonService.GetWeeklyMarathonsAsync(userId);
        Marathon? marathon = weeklyMarathons.FirstOrDefault(m => m.Id == id);
        if (marathon is null)
        {
            return NotFound();
        }

        List<Movie> movies = (await _marathonService.GetMoviesForMarathonAsync(id)).ToList();
        List<LeaderboardEntry> leaderboard = (await _marathonService.GetLeaderboardWithUsernamesAsync(id)).ToList();
        MarathonProgress? progress = await _marathonService.GetUserProgressAsync(userId, id);

        bool isLocked = false;
        if (marathon.PrerequisiteMarathonId is int prereqId)
        {
            bool prereqDone = await _marathonService.IsPrerequisiteCompletedAsync(userId, prereqId);
            isLocked = !prereqDone;
        }

        MarathonDetailViewModel viewModel = new MarathonDetailViewModel
        {
            Marathon = marathon,
            Movies = movies,
            Leaderboard = leaderboard,
            Progress = progress,
            IsLocked = isLocked,
            CurrentUserId = userId,
            StatusMessage = TempData["StatusMessage"] as string,
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(int id)
    {
        try
        {
            bool success = await _marathonService.StartMarathonAsync(id);
            TempData["StatusMessage"] = success
                ? "You joined the marathon."
                : "Could not join the marathon.";
        }
        catch (Exception ex)
        {
            TempData["StatusMessage"] = $"Could not join the marathon: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Leaderboard(int id)
    {
        int userId = _currentUserService.UserId;

        IEnumerable<Marathon> weeklyMarathons = await _marathonService.GetWeeklyMarathonsAsync(userId);
        Marathon? marathon = weeklyMarathons.FirstOrDefault(m => m.Id == id);
        if (marathon is null)
        {
            return NotFound();
        }

        List<LeaderboardEntry> leaderboard = (await _marathonService.GetLeaderboardWithUsernamesAsync(id)).ToList();

        MarathonLeaderboardViewModel viewModel = new MarathonLeaderboardViewModel
        {
            Marathon = marathon,
            Leaderboard = leaderboard,
        };

        return View(viewModel);
    }
}
