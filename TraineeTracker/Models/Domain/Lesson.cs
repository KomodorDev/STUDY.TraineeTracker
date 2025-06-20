using System;
using System.Collections.Generic;

namespace TraineeTracker.Models.Domain
{
    public class Lesson
    {
        public required int LessonId { get; set; }

        public required string Title { get; set; }

        public required double EstimatedEffort { get; set; }

        public string LinkUrl { get; set; }

        public bool IsInactive { get; set; }

        public List<Feedback> Feedbacks { get; set; } = new();

        public Lesson() {}
    }
}
