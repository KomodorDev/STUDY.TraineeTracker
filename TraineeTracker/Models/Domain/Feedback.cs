using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents feedback left by a trainee for a specific lesson, including difficulty rating,
    /// previous knowledge level, effort spent, and optional comments.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class Feedback {

        /// <summary>
        /// Primary key for the feedback entry.
        /// </summary>
        public int FeedbackId { get; set; }

        /// <summary>
        /// The difficulty level of the lesson as perceived by the trainee.
        /// </summary>
        public required LessonDifficulty Difficulty { get; set; }

        /// <summary>
        /// Timestamp when the feedback was created (UTC).
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The level of prior knowledge the trainee had before starting the lesson.
        /// </summary>
        public required PreviousKnowledgeLevel PreviousKnowledge { get; set; }

        /// <summary>
        /// Estimated hours of effort the trainee spent on the lesson.
        /// </summary>
        public required float HoursOfEffort { get; set; }

        /// <summary>
        /// Optional textual comments provided by the trainee.
        /// </summary>
        public string? Comment { get; set; }

        // --- Beziehungen ---
        public int LessonId { get; set; }
        public required Lesson Lesson { get; set; }

        public required string AuthorId { get; set; }
        public required ApplicationUser Author { get; set; }

        public required ICollection<ApplicationUser> ReadByUsers { get; set; }
    }
}
