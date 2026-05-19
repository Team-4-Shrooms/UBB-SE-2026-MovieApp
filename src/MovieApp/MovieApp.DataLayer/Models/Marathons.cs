namespace MovieApp.DataLayer.Models;

using System;
using MovieApp.DataLayer.Models;

public sealed class Marathon
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? PosterUrl { get; set; }

    public string? Theme { get; set; }

    public int? PrerequisiteMarathonId { get; set; }

    public DateTime? LastFeaturedDate { get; set; }

    public bool IsActive { get; set; }

    public string? WeekScoping { get; set; }

    public bool IsElite => this.PrerequisiteMarathonId.HasValue;

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
