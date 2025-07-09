using Microsoft.AspNetCore.Mvc.Rendering;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Models.ViewModels.Admin {
    public class CreateUserViewModel {
        public ApplicationUserDto User { get; set; } = new ApplicationUserDto();
        public required IEnumerable<SelectListItem> TeachingPlans;
    }
}