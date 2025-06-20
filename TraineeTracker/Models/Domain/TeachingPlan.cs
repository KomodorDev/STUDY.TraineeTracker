using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace TraineeTracker.Models.Domain
{
    public class TeachingPlan
    {
        [Required]
        [JsonPropertyName("TeachingPlanId")]
        public int TeachingPlanId { get; set; }

        [Required]
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("LastUpdated")]
        public DateTime LastUpdated { get; set; }

        [Required]
        public List<Lesson> Lessons { get; set; }

        public TeachingPlan() {}

        public TeachingPlan(int id, string name, DateTime updated, List<Lesson> lessons)
        {
            if (lessons == null || lessons.Count == 0)
                throw new ArgumentException("TeachingPlan braucht eine Lesson min!");

            TeachingPlanId = id;
            Name = name;
            LastUpdated = updated;
            Lessons = lessons;
        }
    }
}
