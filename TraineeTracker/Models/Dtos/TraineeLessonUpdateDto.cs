namespace TraineeTracker.Models.Dtos
{
    public class TraineeLessonUpdateDto {
        public int? TraineeLessonId { get; set; }
        public string? TraineeId { get; set; }
        public int? LessonId { get; set; }
        public required string TargetStateName { get; set; }
        // Only when TargetStateName is Rejected
        public string? RejectionReason { get; set; }
    }
}