using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain {
    public class EmailNotificationSetting {
        [Key]
        public int EmailNotificationSettingsId { get; set; }

        public bool ReceiveSkippedNotifications { get; set; } = false;
        public bool ReceiveOpenNotifications { get; set; } = false;
        public bool ReceiveStartedNotifications { get; set; } = false;
        public bool ReceiveFinishedNotifications { get; set; } = false;
        public bool ReceiveRejectedNotifications { get; set; } = false;
        public bool ReceiveAcceptedNotifications { get; set; } = false;
        public bool ReceiveRatedNotifications { get; set; } = false;
        public bool ReceiveImportChangeNotifications { get; set; } = true;
        // Foreign key
        public string? UserId { get; set; }

        // Navigation property
        public ApplicationUser? User { get; set; }
    }
}
