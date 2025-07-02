using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain {

    public class Feedback {
        public int FeedbackId { get; set; }

        [Range(1, 10)]
        public required int Difficulty {
            get; set;
        }

        public DateTime CreateTime { get; set; } = DateTime.UtcNow;


        public required string PreviousKnowledge { get; set; }

        public required float HoursOfEffort {
            get; set;
        }

        public string? Comment {
            get; set;
        }

        // --- Beziehungen ---

        public int LessonId { get; set; }
        public required Lesson Lesson { get; set; }

        public required string AuthorId { get; set; }
        public required ApplicationUser Author { get; set; }

        public required ICollection<ApplicationUser> ReadByUsers { get; set; }
    }
}
