using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {

    /// <summary>
    /// ViewModel that encapsulates all trainee-related statistics for the statistics dashboard.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Nikita Stefan (stefanni)
    /// </remarks>
    public class TraineeStatisticsViewModel {

        /// <summary>
        /// The selected trainee for which the statistics are shown.
        /// </summary>
        public ApplicationUser? SelectedTrainee { get; set; }

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

        /// <summary>
        /// All lessons in state Finished.
        /// </summary>
        public List<TraineeLessonViewModel>? FinishedLessons { get; set; }

        /// <summary>
        /// All lessons in state Accepted.
        /// </summary>
        public List<TraineeLessonViewModel>? AcceptedLessons { get; set; }

        /// <summary>
        /// All lessons in state Rated.
        /// </summary>
        public List<TraineeLessonViewModel>? RatedLessons { get; set; }

        /// <summary>
        /// All lessons in state Rejected.
        /// </summary>
        public List<TraineeLessonViewModel>? RejectedLessons { get; set; }

        /// <summary>
        /// All lessons in state Open.
        /// </summary>
        public List<TraineeLessonViewModel>? OpenLessons { get; set; }

        /// <summary>
        /// All lessons in state Started.
        /// </summary>
        public List<TraineeLessonViewModel>? StartedLessons { get; set; }

        /// <summary>
        /// All registered processing pauses of the trainee.
        /// </summary>
        public List<ProcessingPause>? ProcessingPauses { get; set; }

        /// <summary>
        /// Combined list of all lessons, annotated with their states (used for charting).
        /// </summary>
        public List<TraineeLessonViewModel>? AllLessons { get; set; }

        /// <summary>
        /// Sum of all weighted efforts for finished, accepted, rated and rejected lessons.
        /// </summary>
        public double CurrentProgress { get; set; }
    }
}
