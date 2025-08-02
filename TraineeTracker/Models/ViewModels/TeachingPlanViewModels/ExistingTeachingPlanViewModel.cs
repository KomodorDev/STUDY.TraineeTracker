namespace TraineeTracker.Models.ViewModels.TeachingPlanViewModels {
    
    // ------------------------------------------------------
    /// <summary>
    /// ViewModel for the ExistingTeachingPlanView
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter
    /// </remarks>
    public class ExistingTeachingPlanViewModel {
        
        public required int TeachingPlanId { get; set; }
        
        public required string Name { get; set; }
        
        public DateTime LastUpdated { get; set; }
        
        public int LessonCount { get; set; }
        
        public int TraineeCount { get; set; }
    }
}
