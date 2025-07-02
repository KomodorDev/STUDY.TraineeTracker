using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {
    public class TraineeStatisticsViewModel {
        public DateTime SnapshotDateTime { get; set; }
        public double? DaysPresentTotal { get; set; }
        public double? DaysPresentTillToday { get; set; }
        public double? LessonDaysCompleted { get; set; }
        public double? LessonDaysOpen { get; set; }
        public double? LessonDaysBuffer { get; set; }
        public double? Speed { get; set; }
        public double? PredictedMissingEstimatedEffortAtEnd { get; set; }
        public double? PredictedMissingActualDays { get; set; }
        public bool IsUpToDate { get; set; }
        public List<TraineeLessonViewModel>? FinishedLessons { get; set; }
        public List<TraineeLessonViewModel>? AcceptedAndRatedLessons { get; set; }
        public List<TraineeLessonViewModel>? RejectedLessons { get; set; }
        public List<TraineeLessonViewModel>? OpenLessons { get; set; }
        public List<TraineeLessonViewModel>? StartedLessons { get; set; }
        public List<ProcessingPause>? ProcessingPauses { get; set; }
    }
}