using System;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {

    // ------------------------------------------------------
    /// <summary>
    /// Provides DTO for FeedbackDashboard
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class FeedbackDashboardDto {

        /// <summary>
        /// Feedback Id as unique Identifyer
        /// </summary>
        public int FeedbackId { get; set; }

        /// <summary>
        /// Boolean to check be set if user has read Feedback
        /// </summary>
        public bool IsReadByCurrentUser { get; set; }

        /// <summary>
        /// Author of the Feedback
        /// </summary>
        public string AuthorName { get; set; } = null!;

        /// <summary>
        /// Title of the Lesson for which the Feedback is
        /// </summary>
        public string LessonTitle { get; set; } = null!;

        /// <summary>
        /// Creation Time of the Feedback
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Feedback Comment
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Difficulty of the Lesson
        /// </summary>
        public LessonDifficulty Difficulty { get; set; }

        /// <summary>
        /// Previous Knowledge of the Trainee
        /// </summary>
        public PreviousKnowledgeLevel PreviousKnowledge { get; set; }

        /// <summary>
        /// Hours of Effort the Trainee had with the Lesson
        /// </summary>
        public float? HoursOfEffort { get; set; }
    }
}
