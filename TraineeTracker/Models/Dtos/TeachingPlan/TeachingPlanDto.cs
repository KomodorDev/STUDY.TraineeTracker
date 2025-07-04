using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    public class TeachingPlanDto {

        // All caes:

        [Required]
        public required IFormFile NewPlanFile { get; set; }

        // Update Case:
        public int? ExistingTeachingPlanId { get; set; }

        // New import case:
        public string? NewPlanName { get; set; }


    }
}
