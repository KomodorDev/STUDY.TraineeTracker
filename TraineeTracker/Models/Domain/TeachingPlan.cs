namespace TraineeTracker.Models.Domain
{
    public class TeachingPlan
    {
        public int TeachingPlanId { get; set; }

        public required string Name { get; set; }

        public required DateTime LastUpdated { get; set; }

        public required List<Lesson> Lessons { get; set; }

        public List<ApplicationUser> AffectedUsers { get; set; } = new();

        public TeachingPlan() {}
    }
}
