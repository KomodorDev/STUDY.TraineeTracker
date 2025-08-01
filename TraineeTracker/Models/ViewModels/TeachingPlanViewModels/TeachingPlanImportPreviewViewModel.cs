using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Models.ViewModels.TeachingPlanViewModels {
    
    // ------------------------------------------------------
    /// <summary>
    /// ViewModel for the TeachingPlanImportPreviewView
    /// Code Ownership: Simon Hinterreiter
    /// </summary>
    public class TeachingPlanImportPreviewViewModel {
        
        public List<LessonDto> NewActiveLessons { get; set; } = new();
        
        public List<LessonDto> NewInactiveLessons { get; set; } = new();
        
        public List<LessonDto> ExistingReactivatedLessons { get; set; } = new();
        
        public List<LessonDto> ExistingDeactivatedLessons { get; set; } = new();
        
        public string? TeachingPlanName { get; set; }
        
        public int TeachingPlanId { get; set; }

        // For Form Resubmission: 
        public string? TempFileName { get; set; }

        // Helper: was JSON already processed?
        public bool IsInitialLoad =>
            !NewActiveLessons.Any() &&
            !NewInactiveLessons.Any() &&
            !ExistingReactivatedLessons.Any() &&
            !ExistingDeactivatedLessons.Any();
    }
}
