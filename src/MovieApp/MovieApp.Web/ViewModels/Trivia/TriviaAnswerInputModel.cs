namespace MovieApp.Web.ViewModels.Trivia
{
    public sealed class TriviaAnswerInputModel
    {
        public int QuestionId { get; set; }
        public string Category { get; set; } = string.Empty;
        public char SelectedOption { get; set; }
    }
}
