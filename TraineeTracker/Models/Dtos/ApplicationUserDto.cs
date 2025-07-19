using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    public class ApplicationUserDto {

        public string Email { get; set; } = "";

        public string Password { get; set; } = "Sopro.2025";
        public string Role { get; set; } = "";
        public int? TeachingPlanId { get; set; }

        // Only for Trainees
        public DateOnly? TraineeStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly? TraineeEndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}