using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using MovieApp.DataLayer.Models;
using MovieApp.Features.Shared.Models;
using MovieApp.Features.TriviaWheel.ViewModels;
using MovieApp.Logic.Interfaces.Services;
using Windows.UI;

namespace MovieApp.Features.TriviaWheel.Views
{
    public sealed partial class TriviaWheelPage : Page
    {
        // Wheel segment configuration — categories match the seeded trivia question data
        private static readonly (string Category, Color Color)[] Segments = new[]
        {
            ("Action",     Color.FromArgb(255, 229,  57,  53)), // red
            ("Sci-Fi",     Color.FromArgb(255, 255, 160,   0)), // amber
            ("Drama",      Color.FromArgb(255,  30, 136,  30)), // green
            ("Horror",     Color.FromArgb(255,  21, 101, 192)), // blue
            ("Thriller",   Color.FromArgb(255, 142,  36, 170)), // purple
            ("Comedy",     Color.FromArgb(255,   0, 172, 193)), // teal
            ("Romance",    Color.FromArgb(255, 233,  30,  99)), // pink
            ("Crime",      Color.FromArgb(255,  69,  90, 100)), // slate
        };

        private TriviaWheelViewModel? _viewModel;
        private string _selectedCategory = string.Empty;

        public TriviaWheelPage()
        {
            this.InitializeComponent();
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel = new TriviaWheelViewModel(
                    App.Services.GetRequiredService<ITriviaService>(),
                    SessionManager.CurrentUserID);

                DrawWheel();
                await _viewModel.InitializeAsync();
                UpdateSpinButton();
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to initialize Trivia Wheel: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot,
                };
                await dialog.ShowAsync();
            }
        }

        // ── Wheel drawing ─────────────────────────────────────────────────────

        private void DrawWheel()
        {
            WheelCanvas.Children.Clear();
            double centerX = 140;
            double centerY = 140;
            double radius = 130;
            int count = Segments.Length;
            double sliceAngle = 360.0 / count;

            for (int i = 0; i < count; i++)
            {
                double startAngle = i * sliceAngle;
                double endAngle = startAngle + sliceAngle;

                var path = CreateSegmentPath(centerX, centerY, radius, startAngle, endAngle, Segments[i].Color);
                WheelCanvas.Children.Add(path);

                // Label
                double midAngle = (startAngle + endAngle) / 2.0 * Math.PI / 180.0;
                double labelRadius = radius * 0.65;
                double labelX = centerX + labelRadius * Math.Sin(midAngle);
                double labelY = centerY - labelRadius * Math.Cos(midAngle);

                var label = new TextBlock
                {
                    Text = Segments[i].Category,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 11,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                };
                Canvas.SetLeft(label, labelX - 30);
                Canvas.SetTop(label, labelY - 8);
                WheelCanvas.Children.Add(label);
            }
        }

        private static Path CreateSegmentPath(
            double cx, double cy, double radius,
            double startDeg, double endDeg,
            Color fillColor)
        {
            double startRad = startDeg * Math.PI / 180.0;
            double endRad = endDeg * Math.PI / 180.0;

            double x1 = cx + radius * Math.Sin(startRad);
            double y1 = cy - radius * Math.Cos(startRad);
            double x2 = cx + radius * Math.Sin(endRad);
            double y2 = cy - radius * Math.Cos(endRad);

            bool largeArc = (endDeg - startDeg) > 180;

            var figure = new PathFigure { StartPoint = new Windows.Foundation.Point(cx, cy) };
            figure.Segments.Add(new LineSegment { Point = new Windows.Foundation.Point(x1, y1) });
            figure.Segments.Add(new ArcSegment
            {
                Point = new Windows.Foundation.Point(x2, y2),
                Size = new Windows.Foundation.Size(radius, radius),
                IsLargeArc = largeArc,
                SweepDirection = SweepDirection.Clockwise,
            });
            figure.Segments.Add(new LineSegment { Point = new Windows.Foundation.Point(cx, cy) });

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return new Path
            {
                Data = geometry,
                Fill = new SolidColorBrush(fillColor),
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 1.5,
            };
        }

        // ── Spin animation ────────────────────────────────────────────────────

        private async void SpinButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel is null || !_viewModel.CanSpin)
            {
                return;
            }

            SpinButton.IsEnabled = false;
            await _viewModel.RecordSpinAsync();

            // Pick a random landing segment
            var random = new Random();
            int segmentIndex = random.Next(Segments.Length);
            _selectedCategory = Segments[segmentIndex].Category;

            double fullRotations = 5 * 360.0;
            double sliceAngle = 360.0 / Segments.Length;
            double targetAngle = fullRotations + (segmentIndex * sliceAngle) + (sliceAngle / 2.0);

            var animation = new DoubleAnimation
            {
                From = 0,
                To = targetAngle,
                Duration = new Duration(TimeSpan.FromSeconds(3)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, WheelRotation);
            Storyboard.SetTargetProperty(animation, "Angle");

            storyboard.Completed += async (_, _) =>
            {
                UpdateSpinButton();
                await LoadQuestionsAsync(_selectedCategory);
            };

            storyboard.Begin();
        }

        private void UpdateSpinButton()
        {
            if (_viewModel is null)
            {
                return;
            }

            bool canSpin = _viewModel.CanSpin && _viewModel.IsTriviaAvailable;
            SpinButton.IsEnabled = canSpin;

            if (!_viewModel.IsTriviaAvailable)
            {
                SpinButton.Content = "No questions available";
            }
            else if (!_viewModel.CanSpin)
            {
                SpinButton.Content = "Already spun";
                NoSpinBanner.Visibility = Visibility.Visible;
            }
        }

        // ── Question loading / display ────────────────────────────────────────

        private async System.Threading.Tasks.Task LoadQuestionsAsync(string category)
        {
            if (_viewModel is null)
            {
                return;
            }

            await _viewModel.LoadQuestionsAsync(category);

            if (_viewModel.TotalQuestions == 0)
            {
                ShowNoQuestionsState(category);
            }
            else
            {
                ShowPlayingPanel();
                ShowCurrentQuestion();
            }
        }

        private void ShowCurrentQuestion()
        {
            if (_viewModel?.CurrentQuestion is null)
            {
                return;
            }

            var question = _viewModel.CurrentQuestion;
            int current = _viewModel.TotalQuestions - (_viewModel.TotalQuestions - _viewModel.Score) + 1;

            QuestionProgressText.Text = $"Question {GetCurrentQuestionNumber()} of {_viewModel.TotalQuestions}  ·  Category: {_selectedCategory}";
            QuestionText.Text = question.QuestionText;

            OptionA.Content = $"A) {question.OptionA}";
            OptionB.Content = $"B) {question.OptionB}";
            OptionC.Content = $"C) {question.OptionC}";
            OptionD.Content = $"D) {question.OptionD}";

            OptionA.IsChecked = false;
            OptionB.IsChecked = false;
            OptionC.IsChecked = false;
            OptionD.IsChecked = false;

            HintButton.IsEnabled = !_viewModel.HintUsed;

            // Apply hint visibility
            ApplyHintVisibility();
        }

        private int GetCurrentQuestionNumber()
        {
            if (_viewModel is null)
            {
                return 1;
            }

            // Derive from score and remaining questions (simple linear tracking)
            // The ViewModel tracks index internally; we count from 1
            var scoreText = _viewModel.ScoreText; // "X/Y"
            return _viewModel.TotalQuestions - CountRemainingQuestions() + 1;
        }

        private int CountRemainingQuestions()
        {
            // Estimate remaining from current question text matching
            // Since we don't expose the index, use TotalQuestions as upper bound
            return _viewModel?.TotalQuestions ?? 0;
        }

        private void ApplyHintVisibility()
        {
            if (_viewModel is null)
            {
                return;
            }

            var hidden = _viewModel.HiddenOptions;
            OptionA.Visibility = hidden.Contains('A') ? Visibility.Collapsed : Visibility.Visible;
            OptionB.Visibility = hidden.Contains('B') ? Visibility.Collapsed : Visibility.Visible;
            OptionC.Visibility = hidden.Contains('C') ? Visibility.Collapsed : Visibility.Visible;
            OptionD.Visibility = hidden.Contains('D') ? Visibility.Collapsed : Visibility.Visible;
        }

        // ── Button handlers ───────────────────────────────────────────────────

        private void HintButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel is null)
            {
                return;
            }

            _viewModel.UseHint();
            HintButton.IsEnabled = false;
            ApplyHintVisibility();
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel is null)
            {
                return;
            }

            char? selected = null;
            if (OptionA.IsChecked == true)
            {
                selected = 'A';
            }
            else if (OptionB.IsChecked == true)
            {
                selected = 'B';
            }
            else if (OptionC.IsChecked == true)
            {
                selected = 'C';
            }
            else if (OptionD.IsChecked == true)
            {
                selected = 'D';
            }

            if (selected is null)
            {
                return;
            }

            _viewModel.SubmitAnswer(selected.Value);

            if (_viewModel.IsSessionComplete)
            {
                ShowResults();
            }
            else
            {
                ShowCurrentQuestion();
            }
        }

        // ── Panel visibility helpers ──────────────────────────────────────────

        private void ShowPlayingPanel()
        {
            IdlePanel.Visibility = Visibility.Collapsed;
            PlayingPanel.Visibility = Visibility.Visible;
            ResultsPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowNoQuestionsState(string category)
        {
            IdlePanel.Visibility = Visibility.Visible;
            PlayingPanel.Visibility = Visibility.Collapsed;
            ResultsPanel.Visibility = Visibility.Collapsed;

            // Re-use the idle panel to show the no-questions message
            var icon = IdlePanel.Child as StackPanel;
        }

        private void ShowResults()
        {
            if (_viewModel is null)
            {
                return;
            }

            IdlePanel.Visibility = Visibility.Collapsed;
            PlayingPanel.Visibility = Visibility.Collapsed;
            ResultsPanel.Visibility = Visibility.Visible;

            bool passed = _viewModel.Score >= _viewModel.TotalQuestions / 2;
            ResultTitle.Text = passed ? "Well done!" : "Better luck next time";
            ScoreText.Text = $"Your score: {_viewModel.ScoreText}";
            ResultDescription.Text = passed
                ? "You earned a reward! Check your inventory to redeem it."
                : "You need at least half correct to earn a reward. Spin again!";

            ResultIcon.Glyph = passed ? "" : "";
        }
    }
}
