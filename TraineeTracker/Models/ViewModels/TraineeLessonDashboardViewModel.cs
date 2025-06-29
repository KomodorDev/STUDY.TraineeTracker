using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels
{
    public class TraineeLessonDashboardViewModel {
        public TraineeStatisticsSnapshot? TraineeStatisticsSnapshot { get; set; }
        public IEnumerable<TraineeLesson>? TraineeLessons { get; set; }
        public TeachingPlan? TeachingPlan { get; set; }

    }
}