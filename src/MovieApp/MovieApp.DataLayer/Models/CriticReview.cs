namespace MovieApp.DataLayer.Models
{
    public class CriticReview
    {
        public string Source { get; set; } = string.Empty;

        public double Score { get; set; }

        public string ScoreDisplay => Score > 0 ? Score.ToString("F1") : string.Empty;

        public bool HasScore => Score > 0;

        public string Headline { get; set; } = string.Empty;

        public string Snippet { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}
