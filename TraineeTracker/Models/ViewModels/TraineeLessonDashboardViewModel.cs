using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels
{
    public class TraineeLessonDashboardViewModel {
        public ApplicationUser? SelectedTrainee { get; set; }
        public TraineeStatisticsSnapshot? TraineeStatisticsSnapshot { get; set; }
        public IEnumerable<TraineeLesson>? TraineeLessons { get; set; }

        public IEnumerable<ApplicationUser>? Trainees { get; set; }
    }
}