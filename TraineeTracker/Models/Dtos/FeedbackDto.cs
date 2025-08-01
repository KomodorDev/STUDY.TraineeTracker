using System.ComponentModel.DataAnnotations;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {

    // ------------------------------------------------------
    /// <summary>
    /// Provides Dto for the Feedback
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public class FeedbackDto {
        
        // ------------------------------------------------------
        /// <summary>
        /// Difficulty of the Lesson
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public required LessonDifficulty Difficulty { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Previous Knowledge of the Trainee
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public required PreviousKnowledgeLevel PreviousKnowledge { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// The hours of effort the Trainee had with the lesson
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public required float HoursOfEffort { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Comment of the Feedback
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string? Comment { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// The TraineeLessonId for the Feedback
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public required int TraineeLessonId { get; set; }
    }
}

