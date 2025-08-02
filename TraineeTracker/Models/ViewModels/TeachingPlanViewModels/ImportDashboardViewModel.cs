namespace TraineeTracker.Models.ViewModels.TeachingPlanViewModels {

    // ------------------------------------------------------
    /// <summary>
    /// ViewModel for the Import Dashboard
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter
    /// </remarks>
    public class ImportDashboardViewModel {
        public List<ExistingTeachingPlanViewModel> ExistingTeachingPlans { get; set; } = new();
    }
}
