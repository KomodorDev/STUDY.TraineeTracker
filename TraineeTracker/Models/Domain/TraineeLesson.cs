namespace TraineeTracker.Models.Domain
{
    public class TraineeLesson {

        // Id or {ClassName}Id -> treated as primary key, not nullable, auto-incremented
        public int TraineeLessonId { get; set; }

        public required string UserId { get; set; }

        public TraineeLessonState State { get; set; } = TraineeLessonState.Open;

        // only required if state is rejected
        public string? RejectionReason { get; set; }

        public DateOnly? DayStarted { get; set; }

        public DateOnly? DayFinished { get; set; }
        
        public required int LessonId { get; set; }

        public required Lesson Lesson { get; set; }

    }
}
