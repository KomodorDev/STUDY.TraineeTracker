using System.ComponentModel.DataAnnotations;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {
    public class ProcessingPauseDto {
        public int? ProcessingPauseId { get; set; }

        [Required]
        public string TraineeId { get; set; } = "";

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}