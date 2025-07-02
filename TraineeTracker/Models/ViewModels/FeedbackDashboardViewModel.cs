using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {
    public class FeedbackDashboardViewModel {
        // Gefilterte, sortierte und paginierte Feedbacks
        public Page<FeedbackDashboardDto> Feedbacks { get; set; } = null!;

        // Filter und Sortierung
        public string ActiveFilter { get; set; } = "all";
        public string ActiveSortBy { get; set; } = "date";
        public bool Ascending { get; set; } = false;

        // Dropdown data
        public IEnumerable<ApplicationUser> ActiveTrainees { get; set; } = [];
        public IEnumerable<Lesson> Lessons { get; set; } = [];

        // Currently selected filters (optional but helpful for form state)
        public string? SelectedTraineeId { get; set; }
        public int? SelectedLessonId { get; set; }

        // Gesamtzahlen für UI
        public int TotalFeedbackCount { get; set; }
        public int ReadFeedbackCount { get; set; }
        public int UnreadFeedbackCount { get; set; }

        // Convenience: Pfeil für Sortierung
        public string SortDirectionSymbol => Ascending ? "↑" : "↓";
    }
}
