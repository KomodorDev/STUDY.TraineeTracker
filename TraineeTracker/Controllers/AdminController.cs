using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels.Admin;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Controllers {
    /// <summary>
    /// Controller for administrative actions in the TraineeTracker application.
    /// Handles user management, dashboard display, and processing pause operations.
    /// Only accessible to users with the "Admin" role.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller {
        /// <summary>
        /// Service for admin-related operations.
        /// </summary>
        private readonly AdminService _adminService;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="adminService">The admin service to use for operations.</param>
        public AdminController(AdminService adminService) {
            _adminService = adminService;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Displays the admin dashboard view with optional filtering, sorting, and paging.
        /// </summary>
        /// <param name="page">The page number to display.</param>
        /// <param name="filterRole">Role filter for users.</param>
        /// <param name="filterStatus">Status filter for users.</param>
        /// <param name="sortBy">Sorting option.</param>
        /// <returns>The dashboard view.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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
        /// <summary>
        /// Displays the view for creating a new user.
        /// </summary>
        /// <returns>The create user view.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        [HttpGet("CreateUser")]
        public async Task<IActionResult> ShowCreateUserView() {
            var viewModel = await _adminService.BuildCreateUserViewModelAsync();
            return View("CreateUser", viewModel);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Handles the creation of a new user.
        /// Validates the input and displays errors if necessary.
        /// </summary>
        /// <param name="viewModel">The view model containing user data.</param>
        /// <returns>Redirects to dashboard on success, otherwise returns the create user view with errors.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUserAsync(CreateUserViewModel viewModel) {

            // 1. No submission yet:
            if (!viewModel.ConfirmSubmission || !ModelState.IsValid) {
                if (!viewModel.ConfirmSubmission) {
                    ModelState.Clear();
                }
                viewModel = await _adminService.FillCreateUserDropdownsAsync(viewModel);
                return View("CreateUser", viewModel);
            }

            // 2. User wants to submit:
            var result = await _adminService.CreateUserAsync(viewModel.User, false, Url);

            if (!result.Succeeded) {
                throw new Exception(result.ErrorMessages.First());
                var modelTask = _adminService.FillCreateUserDropdownsAsync(viewModel);
                foreach (var message in result.ErrorMessages) {
                    ModelState.AddModelError("", message);
                }
                viewModel = await modelTask;
                return View("CreateUser", viewModel);
            }
            throw new Exception("Redirecting to Admin Dashboard");
            return RedirectToAction("ShowAdminDashboardView");
        }

        // ------------------------------------------------------
        /// <summary>
        /// Closes a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to close.</param>
        /// <param name="page">The page number to display.</param>
        /// <param name="filterRole">Role filter for users.</param>
        /// <param name="filterStatus">Status filter for users.</param>
        /// <param name="sortBy">Sorting option.</param>
        /// <returns>The dashboard view or error view if closing fails.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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
        /// <summary>
        /// Displays the view for managing processing pauses for a trainee.
        /// </summary>
        /// <param name="traineeId">The ID of the trainee.</param>
        /// <returns>The manage processing pauses view.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        [HttpGet("ProcessingPauses")]
        public async Task<IActionResult> ShowManageProcessingPausesView(string traineeId) {
            var viewModel = await _adminService.BuildManageProcessingPausesViewModelAsync(traineeId);

            return View("ManageProcessingPauses", viewModel);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Handles the creation of a processing pause for a trainee.
        /// Validates the input and displays errors if necessary.
        /// </summary>
        /// <param name="dto">The processing pause data transfer object.</param>
        /// <returns>Redirects to manage processing pauses view on success, otherwise returns the view with errors.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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
        /// <summary>
        /// Handles the editing of a processing pause for a trainee.
        /// Validates the input and displays errors if necessary.
        /// </summary>
        /// <param name="dto">The processing pause data transfer object.</param>
        /// <returns>Redirects to manage processing pauses view on success, otherwise returns the view with errors.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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
        /// <summary>
        /// Handles the deletion of a processing pause.
        /// </summary>
        /// <param name="processingPauseId">The ID of the processing pause to delete.</param>
        /// <returns>Redirects to manage processing pauses view on success, otherwise returns NotFound.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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
        /// <summary>
        /// Displays the error view.
        /// </summary>
        /// <returns>The error view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }

        // ------------------------------------------------------
    }
}