using System.Text.Json.Serialization;

namespace MovieApp.Logic.Models.DTOs;

public sealed class OmdbResponseDto
{
    [JsonPropertyName("imdbID")]
    public string ImdbId { get; set; } = string.Empty;

    [JsonPropertyName("Ratings")]
    public List<OmdbRatingDto> Ratings { get; set; } = new();
}

public sealed class OmdbRatingDto
{
    [JsonPropertyName("Source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("Value")]
    public string Value { get; set; } = string.Empty;
}

public sealed class OmdbContextDto
{
    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("Year")]
    public string Year { get; set; } = string.Empty;

    [JsonPropertyName("Director")]
    public string Director { get; set; } = string.Empty;
}
