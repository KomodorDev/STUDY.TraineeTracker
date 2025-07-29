using Microsoft.AspNetCore.Mvc.Rendering;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Models.ViewModels.Admin {
    /// <summary>
    /// ViewModel for creating a new user in the admin interface.
    /// Contains user details, selectable roles, teaching plans, and a confirmation flag.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class CreateUserViewModel {
        public ApplicationUserDto User { get; set; } = new ApplicationUserDto();
        public IEnumerable<SelectListItem> Roles = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> TeachingPlans = Enumerable.Empty<SelectListItem>();
        public bool ConfirmSubmission { get; set; } = false;
    }
}