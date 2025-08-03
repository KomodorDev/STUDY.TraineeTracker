using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents an application user in the TraineeTracker system, extending the ASP.NET IdentityUser.
    /// Contains properties for trainee and mentor roles, notification settings, and related domain entities.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class ApplicationUser : IdentityUser {
        
        /// <summary>
        /// Indicates whether the user account is closed.
        /// </summary>
        public bool IsClosed { get; set; } = false;

        /// <summary>
        /// The email notification settings associated with the user.
        /// </summary>
        public required EmailNotificationSetting EmailNotificationSetting { get; set; }

        // Trainee properties

        /// <summary>
        /// The start date of the trainee period.
        /// </summary>
        public DateOnly? TraineeStartDate { get; set; }

        /// <summary>
        /// The end date of the trainee period.
        /// </summary>
        public DateOnly? TraineeEndDate { get; set; }

        /// <summary>
        /// The ID of the associated teaching plan.
        /// </summary>
        public int? TeachingPlanId { get; set; }

        /// <summary>
        /// The teaching plan associated with the trainee.
        /// </summary>
        public TeachingPlan? TeachingPlan { get; set; }

        /// <summary>
        /// The statistics snapshot for the trainee.
        /// </summary>
        public TraineeStatisticsSnapshot? TraineeStatisticsSnapshot { get; set; }

        /// <summary>
        /// The collection of processing pauses for the trainee.
        /// </summary>
        public ICollection<ProcessingPause> ProcessingPauses { get; set; } = new List<ProcessingPause>();

        /// <summary>
        /// The collection of lessons assigned to the trainee.
        /// </summary>
        public ICollection<TraineeLesson> TraineeLessons { get; set; } = new List<TraineeLesson>();

        /// <summary>
        /// The collection of feedback written by the trainee.
        /// </summary>
        public ICollection<Feedback> WrittenFeedbacks { get; set; } = new List<Feedback>();

        // Mentor properties

        /// <summary>
        /// The collection of feedback read by the mentor.
        /// </summary>
        public ICollection<Feedback> ReadFeedbacks { get; set; } = new List<Feedback>();

        /// <summary>
        /// The list of trainees last selected by the mentor.
        /// </summary>
        public List<ApplicationUser> LastSelectedTrainees { get; set; } = new List<ApplicationUser>();
    }
}