namespace TraineeTracker.Models.Dtos
{
    public class FeedbackDto {
        public required int TraineeLessonId { get; set; }
        public int EffortDays { get; set; }
        public string? PriorKnowledge { get; set; }
        public string? Difficulty { get; set; }
        public string? Comment { get; set; }
    }
}