namespace TraineeTracker.Models.Dtos {
    public class NotificationSettingDto {
        public bool ReceiveSkippedNotifications { get; set; }
        public bool ReceiveOpenNotifications { get; set; }
        public bool ReceiveStartedNotifications { get; set; }
        public bool ReceiveFinishedNotifications { get; set; }
        public bool ReceiveRejectedNotifications { get; set; }
        public bool ReceiveAcceptedNotifications { get; set; }
        public bool ReceiveRatedNotifications { get; set; }

        public string? UserId { get; set; } // Optional, falls du das aus Claims ableitest
    }
}
