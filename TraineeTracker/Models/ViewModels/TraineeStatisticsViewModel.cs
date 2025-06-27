namespace TraineeTracker.Models.ViewModels
{
    public class TraineeStatisticsViewModel
    {
        public DateTime SnapshotDate { get; set; }
        public double DaysPresent { get; set; }
        public double LessonDaysCompleted { get; set; }
        public double LessonDaysOpen { get; set; }
        public double LessonDaysBuffer { get; set; }
        public double Speed { get; set; }
        public double DaysBufferPredicted { get; set; }
    }
}