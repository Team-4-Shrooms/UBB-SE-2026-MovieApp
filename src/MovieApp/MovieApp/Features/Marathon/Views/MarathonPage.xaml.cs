namespace MovieApp.Features.Marathon.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.DataLayer.Models;
using MovieApp.Features.Marathon.ViewModels;
using MovieApp.Features.Shared.Models;
using MovieApp.Features.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Represents the marathons page, allowing users to browse marathons,
/// join them, and complete trivia challenges for movies.
/// </summary>
public sealed partial class MarathonPage : Page
{
    private MarathonTriviaViewModel? _triviaViewModel;
    private int _currentMovieId;

    public MarathonPage()
    {
        this.InitializeComponent();

        this.Loaded += async (_, _) =>
        {
            int userId = SessionManager.CurrentUserID;
            await this.ViewModel.LoadAsync(userId);
        };
    }

    public MarathonViewModel ViewModel { get; } =
        App.Services.GetRequiredService<MarathonViewModel>();

    private async void MarathonCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement frameworkElement)
        {
            return;
        }

        if (frameworkElement.Tag is not Marathon marathon)
        {
            return;
        }

        await this.ViewModel.SelectMarathonAsync(marathon);

        this.DetailTitle.Text = marathon.Title;
        this.DetailTheme.Text = marathon.Theme ?? string.Empty;
        this.LeaderboardSubtitle.Text = $"{this.ViewModel.Leaderboard.Count} participants";

        this.RefreshProgressBar();

        this.LockedBanner.Visibility = this.ViewModel.IsLocked
            ? Visibility.Visible : Visibility.Collapsed;
        this.JoinButton.Visibility = this.ViewModel.ShowJoinButton
            ? Visibility.Visible : Visibility.Collapsed;
        this.JoinPromptText.Visibility = this.ViewModel.ShowJoinButton
            ? Visibility.Visible : Visibility.Collapsed;

        if (this.ViewModel.IsJoined)
        {
            this.ShowMovieList();
        }
        else if (!this.ViewModel.IsLocked)
        {
            this.ShowJoinPrompt();
        }
        else
        {
            this.ShowIdle();
        }

        this.DetailPanel.Visibility = Visibility.Visible;
    }

    private async void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.ViewModel.SelectedMarathon is null)
        {
            return;
        }

        bool success = await this.ViewModel.JoinMarathonAsync(this.ViewModel.SelectedMarathon.Id);

        if (success)
        {
            this.JoinButton.Visibility = Visibility.Collapsed;
            this.JoinPromptText.Visibility = Visibility.Collapsed;
            this.RefreshProgressBar();
            this.ShowMovieList();
        }
        else
        {
            this.LockedBanner.Visibility = Visibility.Visible;
        }
    }

    private async void LogButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not int movieId)
        {
            return;
        }

        this._currentMovieId = movieId;

        var movie = this.ViewModel.Movies.FirstOrDefault(m => m.MovieId == movieId);
        this.QuizMovieTitle.Text = movie?.Title ?? "Movie";

        var triviaRepository = App.Services.GetService<MovieApp.DataLayer.Interfaces.Repositories.ITriviaRepository>();
        if (triviaRepository is null)
        {
            await ShowInfoAsync("Not available", "Trivia repository is not configured.");
            return;
        }

        this._triviaViewModel = new MarathonTriviaViewModel(triviaRepository);

        try
        {
            await this._triviaViewModel.StartAsync(movieId);
            this.ShowPlaying();
            this.RefreshQuizUi();
        }
        catch (InvalidOperationException ex)
        {
            await ShowInfoAsync("Cannot start quiz", ex.Message);
        }
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (this._triviaViewModel is null)
        {
            return;
        }

        RadioButton? selected = new[] { this.OptionA, this.OptionB, this.OptionC, this.OptionD }
            .FirstOrDefault(r => r.IsChecked == true);

        if (selected?.Tag is not char option)
        {
            return;
        }

        this._triviaViewModel.SubmitAnswer(option);

        foreach (RadioButton radioButton in new[] { this.OptionA, this.OptionB, this.OptionC, this.OptionD })
        {
            radioButton.IsChecked = false;
        }

        this.SubmitButton.IsEnabled = false;

        if (this._triviaViewModel.IsComplete)
        {
            this.ShowResult();

            if (this._triviaViewModel.IsPassed && this.ViewModel.SelectedMarathon is not null)
            {
                _ = this.LogPassedMovieAsync(
                    this.ViewModel.SelectedMarathon.Id,
                    this._currentMovieId,
                    this._triviaViewModel.CorrectCount);
            }
        }
        else
        {
            this.RefreshQuizUi();
        }
    }

    private async Task LogPassedMovieAsync(int marathonId, int movieId, int correctCount)
    {
        await this.ViewModel.LogMovieAsync(marathonId, movieId, correctCount);
        await this.ViewModel.RefreshAfterMovieLoggedAsync();

        this.LeaderboardSubtitle.Text = $"{this.ViewModel.Leaderboard.Count} participants";
        this.RefreshProgressBar();
    }

    private async void TryAgainButton_Click(object sender, RoutedEventArgs e)
    {
        if (this._triviaViewModel is null)
        {
            return;
        }

        this._triviaViewModel.Reset();
        await this._triviaViewModel.StartAsync(this._currentMovieId);
        this.ShowPlaying();
        this.RefreshQuizUi();
    }

    private void BackToMoviesButton_Click(object sender, RoutedEventArgs e)
    {
        this.ShowMovieList();
    }

    private void RefreshQuizUi()
    {
        if (this._triviaViewModel?.CurrentQuestion is not TriviaQuestion question)
        {
            return;
        }

        this.QuizProgress.Text = this._triviaViewModel.ProgressText;
        this.QuizQuestion.Text = question.QuestionText;
        this.OptionA.Content = question.OptionA;
        this.OptionA.Tag = 'A';
        this.OptionB.Content = question.OptionB;
        this.OptionB.Tag = 'B';
        this.OptionC.Content = question.OptionC;
        this.OptionC.Tag = 'C';
        this.OptionD.Content = question.OptionD;
        this.OptionD.Tag = 'D';
        this.SubmitButton.IsEnabled = false;

        foreach (RadioButton radioButton in new[] { this.OptionA, this.OptionB, this.OptionC, this.OptionD })
        {
            radioButton.Checked += (_, _) => this.SubmitButton.IsEnabled = true;
        }
    }

    private void RefreshProgressBar()
    {
        if (this.ViewModel.Movies.Count == 0)
        {
            this.ProgressBar.Value = 0;
            return;
        }

        int verified = this.ViewModel.Movies.Count(m => m.IsVerified);
        this.ProgressBar.Value = (double)verified / this.ViewModel.Movies.Count;
    }

    private async Task ShowInfoAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private void ShowIdle()
    {
        this.MovieListPanel.Visibility = Visibility.Collapsed;
        this.PlayingPanel.Visibility = Visibility.Collapsed;
        this.ResultPanel.Visibility = Visibility.Collapsed;
    }

    private void ShowJoinPrompt()
    {
        this.MovieListPanel.Visibility = Visibility.Collapsed;
        this.PlayingPanel.Visibility = Visibility.Collapsed;
        this.ResultPanel.Visibility = Visibility.Collapsed;
    }

    private void ShowMovieList()
    {
        this.MovieListPanel.Visibility = Visibility.Visible;
        this.PlayingPanel.Visibility = Visibility.Collapsed;
        this.ResultPanel.Visibility = Visibility.Collapsed;
    }

    private void ShowPlaying()
    {
        this.MovieListPanel.Visibility = Visibility.Collapsed;
        this.PlayingPanel.Visibility = Visibility.Visible;
        this.ResultPanel.Visibility = Visibility.Collapsed;
    }

    private void ShowResult()
    {
        this.MovieListPanel.Visibility = Visibility.Collapsed;
        this.PlayingPanel.Visibility = Visibility.Collapsed;
        this.ResultPanel.Visibility = Visibility.Visible;
        this.ResultText.Text = this._triviaViewModel?.ResultText ?? string.Empty;
        this.TryAgainButton.Visibility = this._triviaViewModel?.IsPassed == false
            ? Visibility.Visible : Visibility.Collapsed;
    }
}
