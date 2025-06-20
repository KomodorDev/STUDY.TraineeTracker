using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain
{
    public class EmailNotificationSetting
    {
        [Key]
        public int EmailNotificationSettingsId { get; set; }

        public bool ReceiveOpenNotifications { get; set; }
        public bool ReceiveStartedNotifications { get; set; }
        public bool ReceiveFinishedNotifications { get; set; }
        public bool ReceiveRejectedNotifications { get; set; }
        public bool ReceiveAcceptedNotifications { get; set; }
        public bool ReceiveRatedNotifications { get; set; }

        // Foreign key
        public string UserId { get; set; } = null!;

        // Navigation property
        public ApplicationUser User { get; set; } = null!;
    }
}
