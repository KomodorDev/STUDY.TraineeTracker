using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels
{
    public class TraineeStatisticsViewModel
    {
        public DateTime SnapshotDateTime { get; set; }
        public double DaysPresent { get; set; }
        public double LessonDaysCompleted { get; set; }
        public double LessonDaysOpen { get; set; }
        public double LessonDaysBuffer { get; set; }
        public double Speed { get; set; }
        public double DaysBufferPredicted { get; set; }
        public List<TraineeLesson>? FinishedLessons { get; set; }
        public List<TraineeLesson>? AcceptedAndRatedLessons { get; set; }
        public List<TraineeLesson>? RejectedLessons { get; set; }
        public List<TraineeLesson>? OpenLessons { get; set; }
        public List<TraineeLesson>? StartedLessons { get; set; }
    }
}