namespace TraineeTracker.Models.Dtos {

    /// <summary>
    /// Data transfer object (DTO) representing a user's email notification preferences
    /// for various lesson and system events within the Trainee Tracker.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class EmailNotificationSettingDto {

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications for skipped lessons.
        /// </summary>
        public bool ReceiveSkippedNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications for newly opened lessons.
        /// </summary>
        public bool ReceiveOpenNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications when a lesson is started.
        /// </summary>
        public bool ReceiveStartedNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications when a lesson is marked as finished.
        /// </summary>

        public bool ReceiveFinishedNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications when a lesson is rejected.
        /// </summary>
        public bool ReceiveRejectedNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications when a lesson is accepted.
        /// </summary>
        public bool ReceiveAcceptedNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications when a lesson is rated.
        /// </summary>
        public bool ReceiveRatedNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications when lessons are changed via import.
        /// </summary>
        public bool ReceiveImportChangeNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to receive notifications when feedback entries change.
        /// </summary>
        public bool ReceiveFeedbackChangeNotifications { get; set; }
    }
}
