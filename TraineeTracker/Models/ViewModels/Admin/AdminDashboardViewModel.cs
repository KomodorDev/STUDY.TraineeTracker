using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels.Admin {
    public class AdminDashboardViewModel {
        public required IEnumerable<ApplicationUser> Users { get; set; }
        public required Dictionary<string, string> UserRoles { get; set; }
    }
}