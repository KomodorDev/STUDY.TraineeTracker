using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    public class ApplicationUserDto {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; }
        public DateTime? TraineeStartDate { get; set; }
        public DateTime? TraineeEndDate { get; set; }
        public int? TeachingPlanId { get; set; }

        // Only for Trainees
        public DateOnly? TraineeStartDate { get; set; }
        public DateOnly? TraineeEndDate { get; set; }
    }
}