using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    public class ApplicationUserDto {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public DateTime? TraineeStartDate { get; set; }
        public DateTime? TraineeEndDate { get; set; }
        public int? TeachingPlanId { get; set; }
    }
}