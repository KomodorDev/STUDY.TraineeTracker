using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace TraineeTracker.Models.Domain
{
    public class TeachingPlan
    {
        public int TeachingPlanId { get; set; }

        public required string Name { get; set; }

        public required DateTime LastUpdated { get; set; }

        public required List<Lesson> Lessons { get; set; };

        public List<ApplicationUser> affectedUsers { get; set; } = new();

        public TeachingPlan() {}
    }
}
