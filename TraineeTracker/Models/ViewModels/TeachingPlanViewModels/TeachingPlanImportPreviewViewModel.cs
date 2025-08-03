using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Models.ViewModels.TeachingPlanViewModels {
    
    // ------------------------------------------------------
    /// <summary>
    /// ViewModel for the TeachingPlanImportPreviewView
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter
    /// </remarks>
    public class TeachingPlanImportPreviewViewModel {
        
        /// <summary>
        /// A list of new Active Lessons
        /// </summary>
        public List<LessonDto> NewActiveLessons { get; set; } = new();
        
        /// <summary>
        /// A list of new Inactive Lessons
        /// </summary>
        public List<LessonDto> NewInactiveLessons { get; set; } = new();
        
        /// <summary>
        /// A List of Existing Reavtivated Lessons
        /// </summary>
        public List<LessonDto> ExistingReactivatedLessons { get; set; } = new();
        
        /// <summary>
        /// A List of Existong Deactivated Lessons
        /// </summary>
        public List<LessonDto> ExistingDeactivatedLessons { get; set; } = new();
        
        /// <summary>
        /// The name of the TeachingPlan being displayed in the preview
        /// </summary>
        public string? TeachingPlanName { get; set; }
        
        /// <summary>
        /// The Id of the TeachingPlan
        /// </summary>
        public int TeachingPlanId { get; set; }

        /// <summary>
        /// The Form for Resubmission
        /// </summary>
        public string? TempFileName { get; set; }

        /// <summary>
        /// Helper: was JSON already processed?
        /// </summary>
        public bool IsInitialLoad =>
            !NewActiveLessons.Any() &&
            !NewInactiveLessons.Any() &&
            !ExistingReactivatedLessons.Any() &&
            !ExistingDeactivatedLessons.Any();
    }
}
