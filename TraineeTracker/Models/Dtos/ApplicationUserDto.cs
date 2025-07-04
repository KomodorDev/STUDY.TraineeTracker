using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    public class ApplicationUserDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
        public int? TeachingPlanId { get; set; }

        // Only for Trainees
        public DateOnly? TraineeStartDate { get; set; }
        public DateOnly? TraineeEndDate { get; set; }
    }
}