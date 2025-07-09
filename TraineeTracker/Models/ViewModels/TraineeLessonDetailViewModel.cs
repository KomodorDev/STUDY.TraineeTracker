// class by schleale

using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {
    public class TraineeLessonDetailViewModel {
        public required TraineeLesson TraineeLesson { get; set; }
        public required Lesson Lesson { get; set; }
        public required IEnumerable<TraineeLessonLogEntry> LogEntries { get; set; }
        public required IEnumerable<Feedback> Feedbacks { get; set; }
        public required IEnumerable<string> AllowedStateTransitions { get; set; }
    }
}