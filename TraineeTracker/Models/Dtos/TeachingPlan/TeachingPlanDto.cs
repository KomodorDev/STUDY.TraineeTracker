using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {
    public class TeachingPlanDto {

        // Case: Initial upload (Check preview)
        public IFormFile? NewPlanFile { get; set; }

        // Case: Update existing plan
        public int? ExistingTeachingPlanId { get; set; }

        // Case: Create new plan
        public string? NewPlanName { get; set; }

        // Case: Confirm final import
        public string? TempFileName { get; set; }
    }
}
