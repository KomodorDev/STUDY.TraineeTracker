using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents a user's email notification preferences for various lesson-related events and plan changes.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>

    public class EmailNotificationSetting {

        /// <summary>
        /// Primary key for the email notification setting entry.
        /// </summary>
        [Key]
        public int EmailNotificationSettingsId { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is skipped.
        /// </summary>
        public bool ReceiveSkippedNotifications { get; set; } = false;

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is marked as open.
        /// </summary>
        public bool ReceiveOpenNotifications { get; set; } = false;

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is started.
        /// </summary>
        public bool ReceiveStartedNotifications { get; set; } = false;

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is finished.
        /// </summary>
        public bool ReceiveFinishedNotifications { get; set; } = false;

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is rejected by the mentor.
        /// </summary>
        public bool ReceiveRejectedNotifications { get; set; } = false;

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is accepted by the mentor.
        /// </summary>
        public bool ReceiveAcceptedNotifications { get; set; } = false;

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson has received a rating/feedback.
        /// </summary>
        public bool ReceiveRatedNotifications { get; set; } = false;

        /// <summary>
        /// Indicates whether the user wants to be notified about teaching plan import changes (e.g. lessons added or removed).
        /// </summary>
        public bool ReceiveImportChangeNotifications { get; set; } = false;

        /// <summary>
        /// Indicates whether the user wants to be notified about feedback changes (e.g. a feedback was
        /// </summary>
        public bool ReceiveFeedbackChangeNotifications { get; set; } = false;

        /// <summary>
        /// Foreign key to the user this setting belongs to.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Navigation property to the associated user.
        /// </summary>
        public ApplicationUser? User { get; set; }
    }
}
