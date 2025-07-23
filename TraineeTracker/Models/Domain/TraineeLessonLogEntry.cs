namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents a single log entry that records a state change of a trainee lesson.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class TraineeLessonLogEntry {

        /// <summary>
        /// Primary key for the log entry.
        /// </summary>
        public int TraineeLessonLogEntryId { get; set; }

        // +++++++++++++++
        /// <summary>
        /// The ID of the associated trainee lesson.
        /// </summary>
        public required int TraineeLessonId { get; init; }

        /// <summary>
        /// The name of the lesson at the time of the log entry.
        /// </summary>
        public required string LessonName { get; init; }

        // +++++++++++++++
        /// <summary>
        /// The ID of the user who triggered the state change.
        /// </summary>
        public required string UserId { get; init; }

        /// <summary>
        /// The display name of the user who triggered the state change.
        /// </summary>
        public required string UserName { get; init; }

        // +++++++++++++++
        /// <summary>
        /// The old lesson state before the change (as a string).
        /// </summary>
        public required string OldState { get; init; }

        /// <summary>
        /// The new lesson state after the change (as a string).
        /// </summary>
        public required string NewState { get; init; }

        // +++++++++++++++
        /// <summary>
        /// The timestamp of when the change occurred.
        /// </summary>
        public required DateTime Timestamp { get; init; }
    }
}
