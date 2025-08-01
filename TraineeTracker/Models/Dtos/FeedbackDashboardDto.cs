using System;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {

    // ------------------------------------------------------
    /// <summary>
    /// Provides DTO for FeedbackDashboard
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public class FeedbackDashboardDto {

        // ------------------------------------------------------
        /// <summary>
        /// Feedback Id as unique Identifyer
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public int FeedbackId { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Boolean to check be set if user has read Feedback
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public bool IsReadByCurrentUser { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Author of the Feedback
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string AuthorName { get; set; } = null!;

        // ------------------------------------------------------
        /// <summary>
        /// Title of the Lesson for which the Feedback is
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string LessonTitle { get; set; } = null!;

        // ------------------------------------------------------
        /// <summary>
        /// Creation Time of the Feedback
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public DateTime CreateTime { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Feedback Comment
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string? Comment { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Difficulty of the Lesson
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public LessonDifficulty Difficulty { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Previous Knowledge of the Trainee
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public PreviousKnowledgeLevel PreviousKnowledge { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Hours of Effort the Trainee had with the Lesson
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public float? HoursOfEffort { get; set; }
    }
}
