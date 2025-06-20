using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace TraineeTracker.Models.Domain
{
    public class TeachingPlan
    {
        public int TeachingPlanId { get; set; }

        public string Name { get; set; }

        public DateTime LastUpdated { get; set; }

        public List<Lesson> Lessons { get; set; } = new();

        [JsonIgnore]
        public List<ApplicationUser> affectedUsers { get; set; } = new();

        public TeachingPlan() {}
    }
}
