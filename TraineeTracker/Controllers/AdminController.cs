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
            return View();
        }

        [HttpGet("/CreateUser")]
        public IActionResult ShowCreateUserView() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(ApplicationUserDto dto) {
            if (!ModelState.IsValid) {
                return View(dto);
            }
            var result = await _adminService.CreateUserAsync(dto);
            if (result.Succeeded) {
                return RedirectToAction("ShowAdminDashboardView");
            }
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error.Description);
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CloseUserAsync(string userId) {
            var success = await _adminService.SetIsClosedAsync(userId, true);
            if (!success) {
                return NotFound();
            }
            return RedirectToAction("ShowAdminDashboardView");
        }

        [HttpPost]
        public async Task<IActionResult> OpenUserAsync(string userId) {
            var success = await _adminService.SetIsClosedAsync(userId, false);
            if (!success) {
                return NotFound();
            }
            return RedirectToAction("ShowAdminDashboardView");
        }

        [HttpGet("/CreateProcessingPause")]
        public IActionResult ShowCreateProcessingPauseView(string traineeId) {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcessingPauseAsync(ProcessingPauseDto dto) {
            if (!ModelState.IsValid) {
                return View(dto);
            }
            var result = await _adminService.CreateProcessingPauseAsync(dto);
            if (!result.Succeeded) {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(dto);
            }
            return RedirectToAction("ShowAdminDashboardView");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }
    }
}