namespace TraineeTracker.Models.Domain {
    public class TraineeLessonLogEntry {
        public int TraineeLessonLogEntryId { get; set; } // Primary key for EF

        // Lesson:
        public required int TraineeLessonId {
            get; init;
        }
        public required string LessonName { get; init; }

        // Editing User:
        public required string UserId {
            get; init;
        }
        public required string UserName { get; init; }

        // Old State and new State (as Strings -> Better for logging)
        public required string OldState {
            get; init;
        }
        public required string NewState { get; init; }

        // Time of change
        public required DateTime Timestamp {
            get; init;
        }


    }
}
