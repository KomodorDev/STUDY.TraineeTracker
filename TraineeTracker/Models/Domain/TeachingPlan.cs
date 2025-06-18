using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Tutorial_Project.Models
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

        public TeachingPlan() {}

        public TeachingPlan(int id, string name, DateTime updated)
        {
            TeachingPlanId = id;
            Name = name;
            LastUpdated = updated;
        }
    }
}
