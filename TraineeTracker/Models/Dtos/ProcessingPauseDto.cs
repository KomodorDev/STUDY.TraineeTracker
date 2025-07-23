using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    public class ProcessingPauseDto {
        public int? ProcessingPauseId { get; set; }

        [Required]
        public string TraineeId { get; set; } = "";

        [Required]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required]
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}