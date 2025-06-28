namespace TraineeTracker.Models.Dtos
{
    public class FeedbackViewModel
    {
        public DateTime SentAt     { get; set; }
        public string   AuthorName { get; set; } = string.Empty;
        public string   Comment    { get; set; } = string.Empty;
    }
}