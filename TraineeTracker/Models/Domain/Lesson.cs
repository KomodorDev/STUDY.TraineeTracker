using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Tutorial_Project.Models
{
    public class Lesson
    {
        [Required]
        [JsonPropertyName("LessonId")]
        public int LessonId { get; set; }

        [Required]
        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [Required]
        [JsonPropertyName("EstimatedEffort")]
        public double EstimatedEffort { get; set; }

        [Required]
        [Url]
        [JsonPropertyName("LinkUrl")]
        public string LinkUrl { get; set; }

        [Required]
        [JsonPropertyName("IsDepracated")]
        public bool IsDepracated { get; set; }

        public Lesson() {}

        public Lesson(int id, string title, string url, double time)
        {
            LessonId = id;
            Title = title;
            EstimatedEffort = time;
            LinkUrl = url;
        }
    }
}
