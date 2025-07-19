using Microsoft.AspNetCore.Mvc.Rendering;
using TraineeTracker.Models.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TraineeTracker.Models.ViewModels.Admin {
    public class CreateUserViewModel {

        [ValidateNever]
        public ApplicationUserDto User { get; set; } = new ApplicationUserDto();
        public IEnumerable<SelectListItem> Roles = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> TeachingPlans = Enumerable.Empty<SelectListItem>();
        public bool ConfirmSubmission { get; set; } = false;
    }
}