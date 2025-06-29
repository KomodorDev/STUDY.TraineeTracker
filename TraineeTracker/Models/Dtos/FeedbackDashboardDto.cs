namespace TraineeTracker.Models.Dtos
{
    public class FeedbackDashboardDto
    {
        public int FeedbackId { get; set; }
        public string AuthorName { get; set; } = null!;
        public DateTime SendDate   { get; set; }
        public string? Comment     { get; set; }
    }
}

