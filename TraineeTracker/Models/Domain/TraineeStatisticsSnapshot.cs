namespace TraineeTracker.Models.Domain {
    
    /// <summary>
    /// Represents a snapshot of a trainee's performance statistics at a specific point in time.
    /// Includes calculated effort, progress metrics, and forecasted outcomes.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Nikita Stefan (stefanni)
    /// </remarks>
    public class TraineeStatisticsSnapshot {

        /// <summary>
        /// Unique identifier for the snapshot.
        /// </summary>
        public int TraineeStatisticsSnapshotId { get; set; }

        /// <summary>
        /// ID of the trainee this snapshot belongs to.
        /// </summary>
        public required string TraineeId { get; set; }

        /// <summary>
        /// Navigation property for the trainee.
        /// </summary>
        public required ApplicationUser Trainee { get; set; }

        /// <summary>
        /// Timestamp indicating when the snapshot was created.
        /// </summary>
        public DateTime SnapshotDateTime { get; set; }

        /// <summary>
        /// Total present days between trainee start and end date.
        /// </summary>
        public double? DaysPresentTotal { get; set; }

        /// <summary>
        /// Present days from start date until today.
        /// </summary>
        public double? DaysPresentTillToday { get; set; }

        /// <summary>
        /// Number of lesson days that have been completed.
        /// </summary>
        public double? LessonDaysCompleted { get; set; }

        /// <summary>
        /// Number of lesson days that are still open.
        /// </summary>
        public double? LessonDaysOpen { get; set; }

        /// <summary>
        /// Buffer between present days and completed effort.
        /// </summary>
        public double? LessonDaysBuffer { get; set; }

        /// <summary>
        /// Current progress speed (effort/day).
        /// </summary>
        public double? Speed { get; set; }

        /// <summary>
        /// Forecast of estimated effort that will be missing or left over at the end.
        /// </summary>
        public double? PredictedMissingEstimatedEffortAtEnd { get; set; }

        /// <summary>
        /// Forecast of actual days that will be missing or left over at the end.
        /// </summary>
        public double? PredictedMissingActualDays { get; set; }
        
        /// <summary>
        /// Forecast of remaining present days from today until trainee end date.
        /// </summary>
        public double? PresentDaysInFuture { get; set; }

        /// <summary>
        /// Total estimated effort for all lessons.
        /// </summary>
        public double? TotalEstimatedEffort { get; set; }

        /// <summary>
        /// Indicates whether the data is up to date or based on fallback snapshot.
        /// </summary>
        public bool IsUpToDate { get; set; }
    }
}