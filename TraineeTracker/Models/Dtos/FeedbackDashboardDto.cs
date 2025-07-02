using System;

namespace TraineeTracker.Models.Dtos {
    public class FeedbackDashboardDto {
        public int FeedbackId { get; set; }

        public bool IsReadByCurrentUser { get; set; }

        public string AuthorName { get; set; } = null!;

        public string LessonTitle { get; set; } = null!;

        public DateTime CreateTime { get; set; }

        public string? Comment { get; set; }

        public int? Difficulty { get; set; }

        public string? PreviousKnowledge { get; set; }

        public float? HoursOfEffort { get; set; }
    }
}
