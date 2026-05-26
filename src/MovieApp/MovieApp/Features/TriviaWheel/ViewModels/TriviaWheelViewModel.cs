namespace MovieApp.Features.TriviaWheel.ViewModels;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

/// <summary>
/// Drives the Trivia Wheel feature: spinning the wheel to select a category,
/// answering questions, and granting rewards on successful completion.
/// </summary>
public sealed class TriviaWheelViewModel : INotifyPropertyChanged
{
    private readonly ITriviaService _triviaService;
    private readonly int _currentUserId;

    private List<TriviaQuestion> _questions = new();
    private int _currentQuestionIndex;
    private int _score;
    private bool _canSpin = true;
    private bool _isTriviaAvailable;
    private bool _isPlayingTrivia;
    private bool _isSessionComplete;
    private char? _selectedAnswer;
    private bool _hintUsed;
    private List<char> _hiddenOptions = new();

    public TriviaWheelViewModel(ITriviaService triviaService, int currentUserId)
    {
        _triviaService = triviaService;
        _currentUserId = currentUserId;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Whether the user may spin the wheel this session.</summary>
    public bool CanSpin
    {
        get => _canSpin;
        private set => SetProperty(ref _canSpin, value);
    }

    /// <summary>Whether there are questions available in the data source.</summary>
    public bool IsTriviaAvailable
    {
        get => _isTriviaAvailable;
        private set => SetProperty(ref _isTriviaAvailable, value);
    }

    /// <summary>Whether a trivia session is currently in progress.</summary>
    public bool IsPlayingTrivia
    {
        get => _isPlayingTrivia;
        private set => SetProperty(ref _isPlayingTrivia, value);
    }

    /// <summary>Whether the current trivia session has ended.</summary>
    public bool IsSessionComplete
    {
        get => _isSessionComplete;
        private set => SetProperty(ref _isSessionComplete, value);
    }

    /// <summary>Number of correct answers so far in the current session.</summary>
    public int Score
    {
        get => _score;
        private set
        {
            SetProperty(ref _score, value);
            OnPropertyChanged(nameof(ScoreText));
        }
    }

    /// <summary>Total number of questions in the current session.</summary>
    public int TotalQuestions => _questions.Count;

    /// <summary>The answer option the user has selected for the current question.</summary>
    public char? SelectedAnswer
    {
        get => _selectedAnswer;
        set => SetProperty(ref _selectedAnswer, value);
    }

    /// <summary>Whether the hint has been used for the current question.</summary>
    public bool HintUsed
    {
        get => _hintUsed;
        private set => SetProperty(ref _hintUsed, value);
    }

    /// <summary>Option letters that have been hidden by the hint.</summary>
    public List<char> HiddenOptions
    {
        get => _hiddenOptions;
        private set => SetProperty(ref _hiddenOptions, value);
    }

    /// <summary>The question the user is currently answering, or null when idle.</summary>
    public TriviaQuestion? CurrentQuestion =>
        _currentQuestionIndex < _questions.Count ? _questions[_currentQuestionIndex] : null;

    /// <summary>Human-readable score string, e.g. "2/5".</summary>
    public string ScoreText => $"{Score}/{TotalQuestions}";

    // ── Lifecycle ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Checks question availability and sets <see cref="CanSpin"/> accordingly.
    /// Call once after the page loads.
    /// </summary>
    public async Task InitializeAsync()
    {
        await RefreshTriviaAvailabilityAsync();
        CanSpin = IsTriviaAvailable;
    }

    /// <summary>Queries the service to determine whether any questions exist.</summary>
    public async Task RefreshTriviaAvailabilityAsync()
    {
        var questions = await _triviaService.GetAllQuestionsAsync();
        IsTriviaAvailable = questions.Any();
    }

    // ── Spin / Question loading ────────────────────────────────────────────────

    /// <summary>
    /// Marks the spin as consumed for this session (one spin per page load).
    /// </summary>
    public Task RecordSpinAsync()
    {
        CanSpin = false;
        return Task.CompletedTask;
    }

    /// <summary>Loads questions for the given category and starts a trivia session.</summary>
    public async Task LoadQuestionsAsync(string category)
    {
        var fetched = await _triviaService.GetQuestionsByCategoryAsync(category);
        _questions = fetched.ToList();
        _currentQuestionIndex = 0;
        Score = 0;
        HintUsed = false;
        HiddenOptions = new List<char>();
        SelectedAnswer = null;
        IsPlayingTrivia = _questions.Count > 0;
        IsSessionComplete = false;
        OnPropertyChanged(nameof(CurrentQuestion));
        OnPropertyChanged(nameof(TotalQuestions));
        OnPropertyChanged(nameof(ScoreText));
    }

    // ── Gameplay ───────────────────────────────────────────────────────────────

    /// <summary>Records the user's answer and advances to the next question.</summary>
    public void SubmitAnswer(char selectedOption)
    {
        if (CurrentQuestion is null)
        {
            return;
        }

        if (selectedOption == CurrentQuestion.CorrectOption)
        {
            Score++;
        }

        AdvanceToNextQuestion();
    }

    /// <summary>Eliminates two wrong answer options to help the user.</summary>
    public void UseHint()
    {
        if (HintUsed || CurrentQuestion is null)
        {
            return;
        }

        HintUsed = true;
        HiddenOptions = GetHintOptionsToHide();
    }

    /// <summary>Returns two wrong option letters chosen at random to hide as a hint.</summary>
    public List<char> GetHintOptionsToHide()
    {
        if (CurrentQuestion is null)
        {
            return new List<char>();
        }

        var wrongOptions = new List<char> { 'A', 'B', 'C', 'D' }
            .Where(option => option != CurrentQuestion.CorrectOption)
            .ToList();

        return wrongOptions
            .OrderBy(_ => Random.Shared.Next())
            .Take(2)
            .ToList();
    }

    /// <summary>
    /// Finalises the session and awards a reward when the user answered at least half correctly.
    /// </summary>
    public async Task HandleSessionCompleteAsync()
    {
        IsPlayingTrivia = false;
        IsSessionComplete = true;

        if (TotalQuestions > 0 && Score >= TotalQuestions / 2)
        {
            await GrantRewardAsync();
        }
    }

    /// <summary>Calls the service to grant a new reward to the current user.</summary>
    public async Task GrantRewardAsync()
    {
        await _triviaService.AwardRewardAsync(_currentUserId);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private void AdvanceToNextQuestion()
    {
        _currentQuestionIndex++;
        HintUsed = false;
        HiddenOptions = new List<char>();
        SelectedAnswer = null;

        if (_currentQuestionIndex >= _questions.Count)
        {
            _ = HandleSessionCompleteAsync();
        }

        OnPropertyChanged(nameof(CurrentQuestion));
        OnPropertyChanged(nameof(ScoreText));
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
