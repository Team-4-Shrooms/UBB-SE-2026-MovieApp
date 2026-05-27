using System.ComponentModel.DataAnnotations;

namespace MovieApp.Web.Models;

public class AddCommentInputModel
{
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Comment content is required.")]
    public string Content { get; set; } = string.Empty;
}
