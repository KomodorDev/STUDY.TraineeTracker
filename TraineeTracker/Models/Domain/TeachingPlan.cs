namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents a teaching plan, grouping lessons and enrolled trainees,
    /// and tracking the last update timestamp.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class TeachingPlan {

        /// <summary>
        /// Primary key for the teaching plan entry.
        /// </summary>
        public int TeachingPlanId { get; set; }

        /// <summary>
        /// Human-readable name of the teaching plan.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Timestamp of the last update to this teaching plan.
        /// </summary>
        public required DateTime LastUpdated { get; set; }

        /// <summary>
        /// Collection of lessons included in this teaching plan.
        /// </summary>
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        /// <summary>
        /// Collection of trainees enrolled in this teaching plan.
        /// </summary>
        public ICollection<ApplicationUser> Trainees { get; set; } = new List<ApplicationUser>();
    }
}
