using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos
{
    public class FeedbackDto {
        [Range(1, 10)]
        public int? Difficulty {
            get; set;
        }

        public string? PreviousKnowledge {
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

