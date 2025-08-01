namespace TraineeTracker.Models.ViewModels.TeachingPlanViewModels {
    
    // ------------------------------------------------------
    /// <summary>
    /// ViewModel for the ExistingTeachingPlanView
    /// Code Ownership: Simon Hinterreiter
    /// </summary>
    public class ExistingTeachingPlanViewModel {
        
        public required int TeachingPlanId { get; set; }
        
        public required string Name { get; set; }
        
        public DateTime LastUpdated { get; set; }
        
        public int LessonCount { get; set; }
        
        public int TraineeCount { get; set; }
    }
}
