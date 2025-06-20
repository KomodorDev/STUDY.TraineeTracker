namespace TraineeTracker.Models.Domain
{
    public class TeachingPlan
    {
        public int TeachingPlanId { get; set; }

        public required string Name { get; set; }

        public required DateTime LastUpdated { get; set; }

        public required ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        public ICollection<ApplicationUser> AffectedUsers { get; set; } = new List<ApplicationUser>();

        public TeachingPlan() {}
    }
}
