using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {
    public class TraineeLessonDashboardViewModel {
        public ApplicationUser? SelectedTrainee { get; set; }
        public TraineeStatisticsSnapshot? TraineeStatisticsSnapshot { get; set; }
        public IEnumerable<TraineeLesson>? TraineeLessons { get; set; }

        public IEnumerable<ApplicationUser>? Trainees { get; set; }

        public string ActiveFilter { get; set; } = "all";
        public string SortBy { get; set; } = "sortingIndex_asc";

        // Filter Statistics
        public int CountAll { get; set; }
        public int CountOpen { get; set; }
        public int CountStarted { get; set; }
        public int CountFinished { get; set; }
        public int CountAccepted { get; set; }
        public int CountRejected { get; set; }
        public int CountRated { get; set; }
        public int CountSkipped { get; set; }
    }
}