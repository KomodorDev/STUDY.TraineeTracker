using System;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {
    public class FeedbackDashboardDto {
        public int FeedbackId { get; set; }

        public bool IsReadByCurrentUser { get; set; }

        public string AuthorName { get; set; } = null!;

        public string LessonTitle { get; set; } = null!;

        public DateTime CreateTime { get; set; }

        public string? Comment { get; set; }

        public LessonDifficulty Difficulty { get; set; }

        public PreviousKnowledgeLevel PreviousKnowledge { get; set; }

        public float? HoursOfEffort { get; set; }
    }
}
