using TraineeTracker.Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TraineeTracker.Models.ViewModels {

    /// <summary>
    /// A view model used for displaying detailed information about a specific trainee's trainee lesson.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class TraineeLessonDetailViewModel {

        /// <summary>
        /// The trainee lesson, of which the data is displayed.
        /// </summary>
        public required TraineeLesson TraineeLesson { get; set; }

        /// <summary>
        /// The lesson corresponding to the trainee lesson, used for displaying lesson-specific data like the URL.
        /// </summary>
        public required Lesson Lesson { get; set; }

        /// <summary>
        /// All log entries corresponding to the trainee lesson.
        /// </summary>
        public required IEnumerable<TraineeLessonLogEntry> LogEntries { get; set; }

        /// <summary>
        /// The latest ~10 feedbacks of the lesson corresponding to the trainee lesson.
        /// Every user can see all feedbacks, but trainees can only edit their own.
        /// </summary>
        public required IEnumerable<Feedback> Feedbacks { get; set; }

        /// <summary>
        /// The feedback specific to the lesson + trainee combination of the viewed trainee lesson.
        /// </summary>
        public Feedback? ExistingFeedback { get; set; }

        /// <summary>
        /// The allowed state transitions based on the current state, used for dynamically (de-) activating state change buttons.
        /// </summary>
        public required IEnumerable<string> AllowedStateTransitions { get; set; }

        /// <summary>
        /// The selectable 'Previous Knowledge' options in the feedback form, displayed in the corresponding dropdown when creating or editing a feedback.
        /// </summary>
        public required IEnumerable<SelectListItem> PreviousKnowledgeOptions { get; set; }

        /// <summary>
        /// The selectable 'Difficulty' options in the feedback form, displayed in the corresponding dropdown when creating or editing a feedback.
        /// </summary>
        public required IEnumerable<SelectListItem> DifficultyOptions { get; set; }
    }
}