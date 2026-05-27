using System.ComponentModel.DataAnnotations;

namespace MovieApp.Web.Models;

public class AddReplyInputModel
{
    public int MovieId { get; set; }
    public int ParentCommentId { get; set; }

    [Required(ErrorMessage = "Reply content is required.")]
    public string Content { get; set; } = string.Empty;
}
