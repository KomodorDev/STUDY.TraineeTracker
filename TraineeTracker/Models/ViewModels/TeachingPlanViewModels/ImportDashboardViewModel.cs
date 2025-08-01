namespace TraineeTracker.Models.ViewModels.TeachingPlanViewModels {

    // ------------------------------------------------------
    /// <summary>
    /// ViewModel for the Import Dashboard
    /// Code Ownership: Simon Hinterreiter
    /// </summary>
    public class ImportDashboardViewModel {
        public List<ExistingTeachingPlanViewModel> ExistingTeachingPlans { get; set; } = new();
    }
}
