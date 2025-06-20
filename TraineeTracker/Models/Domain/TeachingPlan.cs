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
        private List<Lesson> Lessons;

        [Required]
        private List<ApplicationUser> affectedUsers;

        public TeachingPlan() {}

        public TeachingPlan(int id, string name, DateTime updated, List<Lesson> lessons, List<ApplicationUser> affectedUsers)
        {
            if (lessons == null || lessons.Count == 0)
                throw new ArgumentException("TeachingPlan braucht eine Lesson min!");

            if(affectedUsers == null)
                throw new ArgumentException("Null Exception");

            TeachingPlanId = id;
            Name = name;
            LastUpdated = updated;
            Lessons = lessons;
            AffectedUsers = affectedUsers;
        }
    }
}
