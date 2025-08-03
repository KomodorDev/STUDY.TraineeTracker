namespace TraineeTracker.Models.ViewModels.TeachingPlanViewModels {

    // ------------------------------------------------------
    /// <summary>
    /// ViewModel for the Import Dashboard
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter
    /// </remarks>
    public class ImportDashboardViewModel {
        
        /// <summary>
        /// A List of already existing TeachingPlans
        /// </summary>
        public List<ExistingTeachingPlanViewModel> ExistingTeachingPlans { get; set; } = new();
    }
}
