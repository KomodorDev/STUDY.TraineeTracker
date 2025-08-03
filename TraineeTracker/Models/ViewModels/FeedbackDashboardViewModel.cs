using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {

    // ------------------------------------------------------
    /// <summary>
    /// Provides DashboardViewModel for FeedbackView
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class FeedbackDashboardViewModel {

        /// <summary>
        /// Feedback Page object
        /// </summary>
        public Page<FeedbackDashboardDto> Feedbacks { get; set; } = null!;

        /// <summary>
        /// Attribute to Filter different Feedbacks in the View
        /// </summary>
        public string ActiveFilter { get; set; } = "all";

        /// <summary>
        /// Attribute to Sort different Feedbacks in the View
        /// </summary>
        public string SortBy { get; set; } = "date_asc";

        /// <summary>
        /// Enumarable of Active Trainees of the Lesson
        /// </summary>
        public IEnumerable<ApplicationUser> ActiveTrainees { get; set; } = [];

        /// <summary>
        /// Enumarable of Lessons
        /// </summary>
        public IEnumerable<Lesson> Lessons { get; set; } = [];

        /// <summary>
        /// Enumarable of the TeachingPlans
        /// </summary>
        public IEnumerable<TeachingPlan> TeachingPlans { get; set; } = [];

        /// <summary>
        /// Currently Selected
        /// </summary>
        public string? SelectedTraineeId { get; set; }

        /// <summary>
        /// Currently Selected
        /// </summary>
        public int? SelectedLessonId { get; set; }

        /// <summary>
        /// Currently Selected
        /// </summary>
        public int? SelectedTeachingPlanId { get; set; }

        /// <summary>
        /// Total for the UI
        /// </summary>
        public int TotalFeedbackCount { get; set; }

        /// <summary>
        /// Feedback count of the already read Feedbacks
        /// </summary>
        public int ReadFeedbackCount { get; set; }

        /// <summary>
        /// Feedback Count of the unread Feedbacks
        /// </summary>
        public int UnreadFeedbackCount { get; set; }

    }
}
