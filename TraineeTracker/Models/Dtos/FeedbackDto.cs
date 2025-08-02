using System.ComponentModel.DataAnnotations;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {

    // ------------------------------------------------------
    /// <summary>
    /// Provides Dto for the Feedback
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class FeedbackDto {
        
        /// <summary>
        /// Difficulty of the Lesson
        /// </summary>
        public required LessonDifficulty Difficulty { get; set; }

        /// <summary>
        /// Previous Knowledge of the Trainee
        /// </summary>
        public required PreviousKnowledgeLevel PreviousKnowledge { get; set; }

        /// <summary>
        /// The hours of effort the Trainee had with the lesson
        /// </summary>
        public required float HoursOfEffort { get; set; }

        /// <summary>
        /// Comment of the Feedback
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// The TraineeLessonId for the Feedback
        /// </summary>
        public required int TraineeLessonId { get; set; }
    }
}

