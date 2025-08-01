using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {

    // ------------------------------------------------------
    /// <summary>
    /// Provides DashboardViewModel for FeedbackView
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public class FeedbackDashboardViewModel {

        // ------------------------------------------------------
        /// <summary>
        /// Feedback Page object
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public Page<FeedbackDashboardDto> Feedbacks { get; set; } = null!;

        // ------------------------------------------------------
        /// <summary>
        /// Attribute to Filter different Feedbacks in the View
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string ActiveFilter { get; set; } = "all";

        // ------------------------------------------------------
        /// <summary>
        /// Attribute to Sort different Feedbacks in the View
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string SortBy { get; set; } = "date_asc";

        // ------------------------------------------------------
        /// <summary>
        /// Enumarable of Active Trainees of the Lesson
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public IEnumerable<ApplicationUser> ActiveTrainees { get; set; } = [];

        // ------------------------------------------------------
        /// <summary>
        /// Enumarable of Lessons
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public IEnumerable<Lesson> Lessons { get; set; } = [];

        // ------------------------------------------------------
        /// <summary>
        /// Enumarable of the TeachingPlans
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public IEnumerable<TeachingPlan> TeachingPlans { get; set; } = [];

        // ------------------------------------------------------
        /// <summary>
        /// Currently Selected
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string? SelectedTraineeId { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Currently Selected
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public int? SelectedLessonId { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Currently Selected
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public int? SelectedTeachingPlanId { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Total for the UI
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public int TotalFeedbackCount { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Feedback count of the already read Feedbacks
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public int ReadFeedbackCount { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Feedback Count of the unread Feedbacks
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public int UnreadFeedbackCount { get; set; }

    }
}
