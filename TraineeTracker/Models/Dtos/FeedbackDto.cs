using System.ComponentModel.DataAnnotations;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {
    public class FeedbackDto {
        
        public required LessonDifficulty Difficulty {
            get; set;
        }

        public required PreviousKnowledgeLevel PreviousKnowledge {
            get; set;
        }

        public float HoursOfEffort {
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

