using System.Collections.Generic;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Web.Models;

public sealed class ScreeningListViewModel
{
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string MoviePosterUrl { get; set; } = string.Empty;
    public List<ScreeningDetailsDto> Screenings { get; set; } = new();
}
