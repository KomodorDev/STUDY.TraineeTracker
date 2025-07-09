using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels.Admin;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly AdminService _adminService;
        private readonly IProcessingPauseRepository _processingPauseRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminService adminService, IApplicationUserRepository applicationUserRepository, IProcessingPauseRepository processingPauseRepository, ITeachingPlanRepository teachingPlanRepository, ILogger<AdminController> logger) {
            _adminService = adminService;
            _processingPauseRepository = processingPauseRepository;
            _logger = logger;
        }

        [HttpGet("/AdminDashboard")]
        public async Task<IActionResult> ShowAdminDashboardView() {
            var viewModel = await _adminService.BuildAdminDashboardViewModelAsync();
            return View("AdminDashboard", viewModel);
        }

        [HttpGet("/CreateUser")]
        public async Task<IActionResult> ShowCreateUserView() {
            var viewModel = await _adminService.BuildCreateUserViewModelAsync();
            return View("CreateUser", viewModel);
        }

        [HttpPost("/CreateUser")]
        public async Task<IActionResult> CreateUserAsync(CreateUserViewModel viewModel) {
            if (!ModelState.IsValid) {
                viewModel = await _adminService.FillCreateUserDropdownsAsync(viewModel);
                return View("CreateUser", viewModel);
            }
            var result = await _adminService.CreateUserAsync(viewModel.User);
            if (result.Succeeded) {
                return RedirectToAction("ShowAdminDashboardView");
            }
            var modelTask = _adminService.FillCreateUserDropdownsAsync(viewModel);
            foreach (var message in result.ErrorMessages) {
                ModelState.AddModelError("", message);
            }
            viewModel = await modelTask;
            return View("CreateUser", viewModel);
        }

        [HttpPost("/CloseUser")]
        public async Task<IActionResult> CloseUserAsync(string userId) {
            var success = await _adminService.CloseUserAsync(userId);
            if (!success) {
                return NotFound();
            }
            return RedirectToAction("ShowAdminDashboardView");
        }

        [HttpGet("/ProcessingPauses")]
        public async Task<IActionResult> ShowManageProcessingPausesView(string traineeId) {
            var trainee = await _adminService.FindByIdWithProcessingPausesAsync(traineeId);
            if (trainee == null) {
                return NotFound();
            }
            return View("ManageProcessingPauses", trainee);
        }

        [HttpGet("/CreateProcessingPause")]
        public async Task<IActionResult> ShowCreateProcessingPauseView(string traineeId) {
            var viewModel = await _adminService.BuildCreateProcessingPauseViewModel(traineeId);
            return View("CreateProcessingPause", viewModel);
        }

        [HttpPost("/CreateProcessingPause")]
        public async Task<IActionResult> CreateProcessingPauseAsync(CreateProcessingPauseViewModel viewModel) {
            if (!ModelState.IsValid) {
                return View("CreateProcessingPause", viewModel);
            }
            var result = await _adminService.CreateProcessingPauseAsync(viewModel.ProcessingPause);
            if (!result.Succeeded) {
                foreach (var message in result.ErrorMessages)
                    ModelState.AddModelError("", message);
                return View("CreateProcessingPause", viewModel);
            }
            return RedirectToAction("ShowManageProcessingPausesView", new { traineeId = viewModel.ProcessingPause.TraineeId });
        }

        [HttpGet("/EditProcessingPause")]
        public async Task<IActionResult> ShowEditProcessingPauseView(int processingPauseId) {
            return View("EditProcessingPause", await _adminService.GetProcessingPauseDtoAsync(processingPauseId));
        }

        [HttpPost("/EditProcessingPause")]
        public async Task<IActionResult> EditProcessingPauseAsync(ProcessingPauseDto dto) {
            if (!ModelState.IsValid) {
                return View("EditProcessingPause", dto);
            }
            await _adminService.UpdateProcessingPauseAsync(dto);
            return RedirectToAction("ShowManageProcessingPausesView", new { traineeId = dto.TraineeId });
        }

        [HttpPost("/DeleteProcessingPause")]
        public async Task<IActionResult> DeleteProcessingPauseAsync(int processingPauseId) {
            var pause = await _processingPauseRepository.FindByIdAsync(processingPauseId);
            if (pause == null) {
                return NotFound();
            }
            var traineeId = pause.TraineeId;
            await _processingPauseRepository.DeleteAsync(pause);
            return RedirectToAction("ShowManageProcessingPausesView", new { TraineeId = traineeId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }
    }
}