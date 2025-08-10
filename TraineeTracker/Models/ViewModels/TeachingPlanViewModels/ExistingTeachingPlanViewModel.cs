using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels.TeachingPlanViewModels {

    // ------------------------------------------------------
    /// <summary>
    /// ViewModel for the ExistingTeachingPlanView
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter
    /// </remarks>
    public class ExistingTeachingPlanViewModel {

        /// <summary>
        /// The Unique Identifier for the ViewModel
        /// </summary>
        public required int TeachingPlanId { get; set; }

        /// <summary>
        /// Name of the TeachingPlan
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The time the teaching plan was last updated
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// The amount of activelessons of the TeachingPlan
        /// </summary>
        public int LessonCount { get; set; }

        /// <summary>
        /// The amount of not-closed trainees assigned to the TeachingPlan
        /// </summary>
        public int TraineeCount { get; set; }

        /// <summary>
        /// The collection of all active lessons that are part of the teaching plan
        /// and not marked as inactive.
        /// </summary>
        public List<Lesson> ActiveLessons { get; set; } = new();

        /// <summary>
        /// The collection of all active trainees assigned to the teaching plan
        /// (trainees that are not closed).
        /// </summary>
        public List<ApplicationUser> ActiveTrainees { get; set; } = new();
    }
}
