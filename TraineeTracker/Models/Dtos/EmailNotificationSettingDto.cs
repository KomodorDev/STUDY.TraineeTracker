namespace TraineeTracker.Models.Dtos {

    /// <summary>
    /// Data Transfer Object (DTO) that represents a user's individual notification preferences.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class EmailNotificationSettingDto {

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is skipped.
        /// </summary>
        public bool ReceiveSkippedNotifications { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is marked as open.
        /// </summary>
        public bool ReceiveOpenNotifications { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is started.
        /// </summary>
        public bool ReceiveStartedNotifications { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is finished.
        /// </summary>
        public bool ReceiveFinishedNotifications { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is rejected by the mentor.
        /// </summary>
        public bool ReceiveRejectedNotifications { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson is accepted by the mentor.
        /// </summary>
        public bool ReceiveAcceptedNotifications { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified when a lesson has received a rating/feedback.
        /// </summary>
        public bool ReceiveRatedNotifications { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified about teaching plan import changes (e.g. lessons added or removed).
        /// </summary>
        public bool ReceiveImportChangeNotifications { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be notified about feedback changes (e.g. a feedback was
        /// </summary>
        public bool ReceiveFeedbackChangeNotifications { get; set; }
    }
}
