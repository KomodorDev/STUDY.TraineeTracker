using System.ComponentModel.DataAnnotations;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {
    public class FeedbackDto {
        [Range(1, 10)]
        public LessonDifficulty? Difficulty {
            get; set;
        }

        public PreviousKnowledgeLevel? PreviousKnowledge {
            get; set;
        }

        public float? HoursOfEffort {
            get; set;
        }

        public string? Comment {
            get; set;
        }

        public required int TraineeLessonId {
            get; set;
        }
    }
}

