using Microsoft.AspNetCore.Mvc.Rendering;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Models.ViewModels.Admin {

    /// <summary>
    /// ViewModel for creating a new user in the admin interface.
    /// Contains user details, selectable roles, teaching plans, and a confirmation flag.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class CreateUserViewModel {

        /// <summary>
        /// The user data to be submitted, including email, role, and optional trainee details.
        /// </summary>
        public ApplicationUserDto User { get; set; } = new ApplicationUserDto();

        /// <summary>
        /// The list of available roles to choose from (e.g. Admin, Mentor, Trainee).
        /// </summary>
        public IEnumerable<SelectListItem> Roles = Enumerable.Empty<SelectListItem>();

        /// <summary>
        /// The list of available teaching plans that can be assigned to a new trainee.
        /// </summary>
        public IEnumerable<SelectListItem> TeachingPlans = Enumerable.Empty<SelectListItem>();

        /// <summary>
        /// Indicates whether the form submission has been confirmed by the admin user.
        /// Used for double-checking actions.
        /// </summary>
        public bool ConfirmSubmission { get; set; } = false;
    }
}