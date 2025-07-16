using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels.Admin;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Controllers {
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller {
        private readonly AdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        // ------------------------------------------------------------------------------------------------------------
        public AdminController(AdminService adminService, ILogger<AdminController> logger) {
            _adminService = adminService;
            _logger = logger;
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpGet("Dashboard")]
        public async Task<IActionResult> ShowAdminDashboardView(string? selectedRole = null, string? selectedStatus = null, string? sortBy = null) {
            var viewModel = await _adminService.BuildAdminDashboardViewModelAsync(selectedRole, selectedStatus, sortBy);
            return View("AdminDashboard", viewModel);
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpGet("CreateUser")]
        public async Task<IActionResult> ShowCreateUserView() {
            var viewModel = await _adminService.BuildCreateUserViewModelAsync();
            return View("CreateUser", viewModel);
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUserAsync(CreateUserViewModel viewModel) {
            if (!ModelState.IsValid) {
                viewModel = await _adminService.FillCreateUserDropdownsAsync(viewModel);
                return View("CreateUser", viewModel);
            }
            var result = await _adminService.CreateUserAsync(viewModel.User);
            if (!result.Succeeded) {
                var modelTask = _adminService.FillCreateUserDropdownsAsync(viewModel);
                foreach (var message in result.ErrorMessages) {
                    ModelState.AddModelError("", message);
                }
                viewModel = await modelTask;
                return View("CreateUser", viewModel);
            }
            return RedirectToAction("ShowAdminDashboardView");
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpPost("CloseUser")]
        public async Task<IActionResult> CloseUserAsync(string userId) {
            var result = await _adminService.CloseUserAsync(userId);
            if (!result.Succeeded) {
                return Error();
            }
            return RedirectToAction("ShowAdminDashboardView");
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpGet("ProcessingPauses")]
        public async Task<IActionResult> ShowManageProcessingPausesView(string traineeId) {
            var trainee = await _adminService.FindByIdWithProcessingPausesAsync(traineeId);
            if (trainee == null) {
                return NotFound();
            }
            return View("ManageProcessingPauses", trainee);
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpGet("CreateProcessingPause")]
        public async Task<IActionResult> ShowCreateProcessingPauseView(string traineeId) {
            var viewModel = await _adminService.BuildCreateProcessingPauseViewModelAsync(traineeId);
            return View("CreateProcessingPause", viewModel);
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpPost("CreateProcessingPause")]
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

        // ------------------------------------------------------------------------------------------------------------
        [HttpGet("EditProcessingPause")]
        public async Task<IActionResult> ShowEditProcessingPauseView(int processingPauseId) {
            var result = await _adminService.GetProcessingPauseDtoAsync(processingPauseId);
            if (!result.Succeeded) {
                return NotFound();
            }
            return View("EditProcessingPause", result.Value);
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpPost("EditProcessingPause")]
        public async Task<IActionResult> EditProcessingPauseAsync(ProcessingPauseDto dto) {
            if (!ModelState.IsValid) {
                return View("EditProcessingPause", dto);
            }
            var result = await _adminService.UpdateProcessingPauseAsync(dto);
            if (!result.Succeeded) {
                foreach (var message in result.ErrorMessages)
                    ModelState.AddModelError("", message);
                return View("EditProcessingPause", dto);
            }
            return RedirectToAction("ShowManageProcessingPausesView", new { traineeId = dto.TraineeId });
        }

        // ------------------------------------------------------------------------------------------------------------
        [HttpPost("DeleteProcessingPause")]
        public async Task<IActionResult> DeleteProcessingPauseAsync(int processingPauseId) {
            var result = await _adminService.DeleteProcessingPauseAsync(processingPauseId);
            if (!result.Succeeded) {
                return NotFound();
            }
            ArgumentNullException.ThrowIfNull(result.Value);
            return RedirectToAction("ShowManageProcessingPausesView", new { TraineeId = result.Value.TraineeId });
        }

        // ------------------------------------------------------------------------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }
    }
}