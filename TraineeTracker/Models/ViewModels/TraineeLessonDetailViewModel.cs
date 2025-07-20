// class by schleale

using TraineeTracker.Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TraineeTracker.Models.ViewModels {
    public class TraineeLessonDetailViewModel {
        public required TraineeLesson TraineeLesson { get; set; }
        public required Lesson Lesson { get; set; }
        public required IEnumerable<TraineeLessonLogEntry> LogEntries { get; set; }
        public required IEnumerable<Feedback> Feedbacks { get; set; }
        public required IEnumerable<string> AllowedStateTransitions { get; set; }
        public Feedback? ExistingFeedback { get; set; }
        public required IEnumerable<SelectListItem> PreviousKnowledgeOptions { get; set; }
        public required IEnumerable<SelectListItem> DifficultyOptions { get; set; }
    }
}