namespace TraineeTracker.Models.Domain
{
    public class TraineeLesson {

        // Key
        public int TraineeLessonId { get; set; }

        public required string TraineeId { get; set; }
        
        public required ApplicationUser Trainee { get; set; }

        public required TraineeLessonState State {
            get; set;
        }

        // only required if state is rejected
        public string? RejectionReason { get; set; }

        public DateOnly? DayStarted { get; set; }

        public DateOnly? DayFinished { get; set; }
        
        public required int LessonId { get; set; }

        public required Lesson Lesson { get; set; }

        }
}
