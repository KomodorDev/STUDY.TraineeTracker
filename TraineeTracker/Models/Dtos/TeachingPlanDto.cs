namespace TraineeTracker.Models.Dtos {
    public class TeachingPlanDto {
        public required int ExistingTeachingPlanId { get; set; }

        public required string NewPlanName { get; set; }

        public required IFormFile NewPlanFile { get; set; }
    }
}
