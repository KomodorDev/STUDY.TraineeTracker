namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents a lesson within a teaching plan, including metadata, external references,
    /// and collections of feedback and trainee progress.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class Lesson {

        /// <summary>
        /// Primary key for the lesson entry.
        /// </summary>
        public int LessonId { get; set; }

        /// <summary>
        /// External Makandra ID used for referencing the lesson in imported JSON.
        /// </summary>
        public required string MakandraId { get; set; }

        /// <summary>
        /// Foreign key to the parent teaching plan.
        /// </summary>
        public required int TeachingPlanId { get; set; }
        
        /// <summary>
        /// Navigation property to the associated <see cref="TeachingPlan"/>.
        /// </summary>
        public TeachingPlan TeachingPlan { get; set; } = null!;

        /// <summary>
        /// Ordering index within the teaching plan.
        /// </summary>
        public required int SortingIndex { get; set; }

        /// <summary>
        /// Title of the lesson.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Estimated effort in hours for completing the lesson.
        /// </summary>
        public required double EstimatedEffort { get; set; }

        /// <summary>
        /// URL to the lesson content or resource.
        /// </summary>
        public required string LinkUrl { get; set; }

        /// <summary>
        /// Indicates whether the lesson is currently inactive (hidden from trainees).
        /// </summary>
        public bool IsInactive { get; set; } = false;

        /// <summary>
        /// Collection of feedback entries associated with this lesson.
        /// </summary>
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        /// <summary>
        /// Collection of trainee progress records for this lesson.
        /// </summary>
        public ICollection<TraineeLesson> TraineeLessons { get; set; } = new List<TraineeLesson>();
    }
}
