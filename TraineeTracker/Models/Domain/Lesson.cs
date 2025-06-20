using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace TraineeTracker.Models.Domain
{
    public class Lesson
    {
        public int LessonId { get; set; }

        public string Title { get; set; }

        public double EstimatedEffort { get; set; }

        public string LinkUrl { get; set; }

        public bool IsDeprecated { get; set; }

        public List<Feedback> Feedbacks { get; set; } = new();

        public Lesson() {}
    }
}
