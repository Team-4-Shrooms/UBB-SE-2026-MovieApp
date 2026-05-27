namespace MovieApp.Logic.Services;

public sealed class ExternalReviewsOptions
{
    public const string SectionName = "ExternalReviews";

    public ProviderKey Omdb { get; set; } = new();

    public ProviderKey Nyt { get; set; } = new();

    public ProviderKey Guardian { get; set; } = new();

    public sealed class ProviderKey
    {
        public string ApiKey { get; set; } = string.Empty;
    }
}
