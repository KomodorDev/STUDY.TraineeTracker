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
        /// The amount of lessons of the TeachingPlan
        /// </summary>
        public int LessonCount { get; set; }
        
        /// <summary>
        /// The amount of trainees assigned to the TeachingPlan
        /// </summary>
        public int TraineeCount { get; set; }
    }
}
