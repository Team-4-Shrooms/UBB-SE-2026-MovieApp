using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Features.Shared.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Features.MovieDetail.ViewModels;

/// <summary>
/// ViewModel for movie detail comments and external reviews.
/// </summary>
public sealed class MovieDetailViewModel : INotifyPropertyChanged
{
    private readonly ICommentService _commentService;

    private readonly IExternalReviewService _externalReviewService;

    private readonly int _currentUserId;

    private Movie? _movie;
    private string _newCommentContent = string.Empty;
    private string _statusMessage = string.Empty;
    private int _replyToCommentId;
    private string _replyContent = string.Empty;

    public MovieDetailViewModel(ICommentService commentService, IExternalReviewService externalReviewService)
    {
        _commentService = commentService;
        _externalReviewService = externalReviewService;
        _currentUserId = SessionManager.CurrentUserID;

        AddCommentCommand = new AsyncRelayCommand(AddCommentAsync);
        SubmitReplyCommand = new AsyncRelayCommand(SubmitReplyAsync);
        StartReplyCommand = new RelayCommand<object?>(OnStartReply);
        CancelReplyCommand = new RelayCommand(OnCancelReply);
    }

    public ObservableCollection<Comment> Comments { get; } = new();

    public ObservableCollection<Comment> RootComments { get; } = new();

    public ObservableCollection<CriticReview> ExternalReviews { get; } = new();

    public Movie? Movie
    {
        get => _movie;
        set => SetProperty(ref _movie, value);
    }

    public string NewCommentContent
    {
        get => _newCommentContent;
        set => SetProperty(ref _newCommentContent, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (SetProperty(ref _statusMessage, value))
            {
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }
    }

    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(_statusMessage);

    public int ReplyToCommentId
    {
        get => _replyToCommentId;
        set => SetProperty(ref _replyToCommentId, value);
    }

    public string ReplyContent
    {
        get => _replyContent;
        set => SetProperty(ref _replyContent, value);
    }

    public ICommand AddCommentCommand { get; }
    public ICommand SubmitReplyCommand { get; }
    public ICommand StartReplyCommand { get; }
    public ICommand CancelReplyCommand { get; }

    public async Task LoadMovieCommentsAsync(Movie movie)
    {
        Movie = movie;
        StatusMessage = string.Empty;
        await LoadCommentsAsync();

        await LoadExternalReviewsAsync();
    }

    private async Task LoadExternalReviewsAsync()
    {
        ExternalReviews.Clear();
        if (Movie == null) return;

        try
        {
            var reviews = await _externalReviewService.GetExternalReviewsAsync(Movie.Title, Movie.ReleaseYear);

            if (reviews != null)
            {
                foreach (var review in reviews)
                {
                    ExternalReviews.Add(review);
                }
            }
        }
        catch (Exception)
        {
        }
    }

    private async Task AddCommentAsync()
    {
        if (Movie == null || string.IsNullOrWhiteSpace(NewCommentContent))
        {
            return;
        }

        try
        {
            await _commentService.AddCommentAsync(_currentUserId, Movie.Id, NewCommentContent);
            NewCommentContent = string.Empty;
            await LoadCommentsAsync();
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task LoadCommentsAsync()
    {
        if (Movie == null)
        {
            return;
        }

        List<Comment> roots = await _commentService.GetCommentsForMovieAsync(Movie.Id);
        ApplyCommentsFromApi(roots);
    }

    private async Task SubmitReplyAsync()
    {
        if (Movie == null || ReplyToCommentId <= 0 || string.IsNullOrWhiteSpace(ReplyContent))
        {
            return;
        }

        try
        {
            await _commentService.AddReplyAsync(_currentUserId, ReplyToCommentId, ReplyContent);
            ReplyContent = string.Empty;
            ReplyToCommentId = 0;
            await LoadCommentsAsync();
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private void OnStartReply(object? param)
    {
        if (param is int commentId)
        {
            ReplyToCommentId = commentId;
        }
        else if (param is long longId)
        {
            ReplyToCommentId = (int)longId;
        }
    }

    private void OnCancelReply()
    {
        ReplyContent = string.Empty;
        ReplyToCommentId = 0;
    }

    private void ApplyCommentsFromApi(IEnumerable<Comment> roots)
    {
        Comments.Clear();
        RootComments.Clear();

        foreach (Comment? root in roots)
        {
            Comment? clone = CloneCommentTree(root);
            RootComments.Add(clone);
            AddToFlat(clone, Comments);
        }
    }

    private static void AddToFlat(Comment comment, ObservableCollection<Comment> flat)
    {
        flat.Add(comment);
        foreach (Comment? reply in comment.Replies)
        {
            AddToFlat(reply, flat);
        }
    }

    private static Comment CloneCommentTree(Comment comment)
    {
        return new Comment
        {
            CommentId = comment.CommentId,
            AuthorId = comment.AuthorId,
            MovieId = comment.MovieId,
            ParentCommentId = comment.ParentCommentId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            Author = comment.Author,
            Movie = comment.Movie,
            Replies = comment.Replies.Select(CloneCommentTree).ToList()
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
