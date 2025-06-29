using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly AdminService _adminService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminService adminService, IApplicationUserRepository applicationUserRepository, ILogger<AdminController> logger) {
            _adminService = adminService;
            _applicationUserRepository = applicationUserRepository;
            _logger = logger;
        }

        [HttpGet("/ManageUsers")]
        public IActionResult ShowAdminDashboardView() {
            return View("AdminDashboardView");
        }

        [HttpGet("/CreateUser")]
        public IActionResult ShowCreateUserView() {
            return View("CreateUserView");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(ApplicationUserDto dto) {
            if (!ModelState.IsValid) {
                return View("CreateUserView", dto);
            }
            var result = await _adminService.CreateUserAsync(dto);
            if (result.Succeeded) {
                return RedirectToAction("ShowAdminDashboardView");
            }
            foreach (var message in result.ErrorMessages) {
                ModelState.AddModelError("", message);
            }
            return View("CreateUserView", dto);
        }

        [HttpPost]
        public async Task<IActionResult> CloseUserAsync(string userId) {
            var success = await _adminService.CloseUserAsync(userId);
            if (!success) {
                return NotFound();
            }
            return RedirectToAction("ShowAdminDashboardView");
        }

        [HttpGet("/CreateProcessingPause")]
        public IActionResult ShowCreateProcessingPauseView(string traineeId) {
            return View("CreateProcessingPauseView");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcessingPauseAsync(ProcessingPauseDto dto) {
            if (!ModelState.IsValid) {
                return View("CreateProcessingPauseView", dto);
            }
            var result = await _adminService.CreateProcessingPauseAsync(dto);
            if (!result.Succeeded) {
                foreach (var message in result.ErrorMessages)
                ModelState.AddModelError("", message);
                return View("CreateProcessingPauseView", dto);
            }
            return RedirectToAction("ShowAdminDashboardView");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }
    }
}