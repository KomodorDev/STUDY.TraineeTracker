namespace TraineeTracker.Models.Domain {
    public class Lesson {

        // Primary Key - Internal
        public int LessonId { get; set; }

        // External Reference (Makandra-ID aus JSON)
        public required string MakandraId { get; set; }

        // TeachingPlan Navigation
        public int TeachingPlanId { get; set; }
        public TeachingPlan TeachingPlan { get; set; } = null!;

        public int SortingIndex { get; set; }

        public required string Title { get; set; }

        public required double EstimatedEffort { get; set; }

        public required string LinkUrl { get; set; }

        public bool IsInactive { get; set; } = false;

        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        public ICollection<TraineeLesson> TraineeLessons { get; set; } = new List<TraineeLesson>();
    }
}
