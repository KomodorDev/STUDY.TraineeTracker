using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels.Admin {
    public class AdminDashboardViewModel {

        // Filtered, sorted, and paginated list of users
        public required Page<ApplicationUser> Users { get; set; }

        // Currently selected filters and sort option (used for form state)
        public string FilterRole { get; set; } = "all";
        public string FilterStatus { get; set; } = "all";
        public string SortBy { get; set; } = "name_asc";

        // Map of user ID to their assigned role (used in the UI)
        public required Dictionary<string, string> UserRoles { get; set; }

        // Count summaries for the UI
        public int TotalUserCount { get; set; }
        public int OpenUserCount { get; set; }
        public int ClosedUserCount { get; set; }

        public int RoleUserCount { get; set; }
        public int AdminCount { get; set; }
        public int MentorCount { get; set; }
        public int TraineeCount { get; set; }
    }
}
