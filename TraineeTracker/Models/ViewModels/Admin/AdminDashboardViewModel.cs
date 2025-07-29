using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels.Admin {
    /// <summary>
    /// ViewModel for the Admin Dashboard, providing user data, filtering, sorting, and user role statistics.
    /// </summary>
    /// <remarks>
    /// Used to display and manage users in the admin dashboard, including filtering by role and status,
    /// sorting, and displaying counts for various user roles and statuses.
    /// Code Ownership: Paul Schweizer (schwepau)
    /// </remarks>
    public class AdminDashboardViewModel {

        /// <summary>Paged list of <see cref="ApplicationUser"/> objects to be displayed.</summary>
        public required Page<ApplicationUser> Users { get; set; }

        /// <summary>Current role filter applied to the user list (default: "all").</summary>
        public string FilterRole { get; set; } = "all";

        /// <summary>Current status filter applied to the user list (default: "all").</summary>
        public string FilterStatus { get; set; } = "all";

        /// <summary>Current sorting option applied to the user list (default: "name_asc").</summary>
        public string SortBy { get; set; } = "name_asc";

        /// <summary>Dictionary mapping user IDs to their roles.</summary>
        public required Dictionary<string, string> UserRoles { get; set; }

        /// <summary>Total number of users.</summary>
        public int TotalUserCount { get; set; }

        /// <summary>Number of users with open status.</summary>
        public int OpenUserCount { get; set; }

        /// <summary>Number of users with closed status.</summary>
        public int ClosedUserCount { get; set; }

        /// <summary>Number of users for the selected role.</summary>
        public int RoleUserCount { get; set; }

        /// <summary>Number of users with the Admin role.</summary>
        public int AdminCount { get; set; }

        /// <summary>Number of users with the Mentor role.</summary>
        public int MentorCount { get; set; }

        /// <summary>Number of users with the Trainee role.</summary>
        public int TraineeCount { get; set; }
    }
}
