namespace MovieApp.DataLayer.Models;
public class Comment
{
    public int CommentId { get; set; }
    public int AuthorId { get; set; }
    public int MovieId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User? Author { get; set; }
    public Movie? Movie { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public int AuthorDisplayId => Author?.Id ?? 0;
    public int ParentCommentDisplayId => ParentComment?.CommentId ?? 0;
    public bool HasParentComment => ParentComment is not null;
}
