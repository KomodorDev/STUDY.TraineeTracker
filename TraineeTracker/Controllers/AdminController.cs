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

        // ------------------------------------------------------
        public AdminController(AdminService adminService, ILogger<AdminController> logger) {
            _adminService = adminService;
            _logger = logger;
        }

        // ------------------------------------------------------
        [HttpGet("Dashboard")]
        public async Task<IActionResult> ShowAdminDashboardView(
            int page = 1,
            string? filterRole = "all",
            string? filterStatus = "open",
            string? sortBy = "role_asc") {

            var viewModel = await _adminService.BuildAdminDashboardViewModelAsync(page, filterRole, filterStatus, sortBy);
            return View("AdminDashboard", viewModel);
        }

        // ------------------------------------------------------
        [HttpGet("CreateUser")]
        public async Task<IActionResult> ShowCreateUserView() {
            var viewModel = await _adminService.BuildCreateUserViewModelAsync();
            return View("CreateUser", viewModel);
        }

        // ------------------------------------------------------
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUserAsync(CreateUserViewModel viewModel) {

            // 1. No submission yet:
            if (!viewModel.ConfirmSubmission || !ModelState.IsValid) {
                viewModel = await _adminService.FillCreateUserDropdownsAsync(viewModel);
                return View("CreateUser", viewModel);
            }

            // 2. User wants to submit:
            var result = await _adminService.CreateUserAsync(viewModel.User, false, Url);

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

        // ------------------------------------------------------
        [HttpPost("CloseUser")]
        public async Task<IActionResult> CloseUserAsync(string userId,
            int page = 1,
            string? filterRole = "all",
            string? filterStatus = "open",
            string? sortBy = "role_asc") {

            var result = await _adminService.CloseUserAsync(userId);
            if (!result.Succeeded) {
                return Error();
            }

            var viewModel = await _adminService.BuildAdminDashboardViewModelAsync(page, filterRole, filterStatus, sortBy);
            return View("AdminDashboard", viewModel);
        }

        // ------------------------------------------------------
        [HttpGet("ProcessingPauses")]
        public async Task<IActionResult> ShowManageProcessingPausesView(string traineeId) {
            var viewModel = await _adminService.BuildManageProcessingPausesViewModelAsync(traineeId);

            return View("ManageProcessingPauses", viewModel);
        }

        // ------------------------------------------------------
        [HttpPost("CreateProcessingPause")]
        public async Task<IActionResult> CreateProcessingPauseAsync(ProcessingPauseDto dto) {

            Console.WriteLine("==> CreateProcessingPauseAsync called");

            if (!ModelState.IsValid) {
                var rebuild = await _adminService.BuildManageProcessingPausesViewModelAsync(dto.TraineeId);

                // Add back the new ProcessingPause
                rebuild!.NewProcessingPause = dto;
                return View("ManageProcessingPauses", rebuild);
            }

            Console.WriteLine("ModelState is valid. Attempting to create ProcessingPause...");
            var result = await _adminService.CreateProcessingPauseAsync(dto);

            if (!result.Succeeded) {
                Console.WriteLine("Creation failed. Rebuilding view model...");

                var rebuild = await _adminService.BuildManageProcessingPausesViewModelAsync(dto.TraineeId);
                if (rebuild is null)
                    return NotFound();

                rebuild.NewProcessingPause = dto;
                foreach (var message in result.ErrorMessages)
                    ModelState.AddModelError("", message);

                return View("ManageProcessingPauses", rebuild);
            }

            Console.WriteLine("Creation succeeded. Redirecting...");
            return RedirectToAction("ShowManageProcessingPausesView", new { traineeId = dto.TraineeId });
        }

        // ------------------------------------------------------
        [HttpPost("EditProcessingPause")]
        public async Task<IActionResult> EditProcessingPauseAsync(ProcessingPauseDto dto) {
            if (!ModelState.IsValid) {
                // We buld a new viewModel and return
                var viewModel = await _adminService.BuildManageProcessingPausesViewModelAsync(dto.TraineeId);
                return View("ManageProcessingPauses", viewModel);
            }

            var result = await _adminService.UpdateProcessingPauseAsync(dto);
            if (!result.Succeeded) {
                foreach (var message in result.ErrorMessages)
                    ModelState.AddModelError("", message);

                var viewModel = await _adminService.BuildManageProcessingPausesViewModelAsync(dto.TraineeId);
                return View("ManageProcessingPauses", viewModel);
            }

            return RedirectToAction("ShowManageProcessingPausesView", new { traineeId = dto.TraineeId });
        }

        // ------------------------------------------------------
        [HttpPost("DeleteProcessingPause")]
        public async Task<IActionResult> DeleteProcessingPauseAsync(int processingPauseId) {
            var result = await _adminService.DeleteProcessingPauseAsync(processingPauseId);
            if (!result.Succeeded) {
                return NotFound();
            }
            ArgumentNullException.ThrowIfNull(result.Value);
            return RedirectToAction("ShowManageProcessingPausesView", new { TraineeId = result.Value.TraineeId });
        }

        // ------------------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }

        // ------------------------------------------------------
    }
}