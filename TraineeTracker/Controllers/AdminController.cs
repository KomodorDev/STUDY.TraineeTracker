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
        public async Task<IActionResult> ShowAdminDashboardView() {
            var users = await _applicationUserRepository.GetAllAsync();
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users) {
                var roles = await _applicationUserRepository.GetRolesAsync(user);
                if (roles.Contains("Admin")) {
                    userRoles[user.Id] = "Admin";
                } else {
                    userRoles[user.Id] = roles.First();
                }
            }
            ViewBag.UserRoles = userRoles;
            return View("AdminDashboard", users);
        }

        [HttpGet("/CreateUser")]
        public IActionResult ShowCreateUserView() {
            return View("CreateUser");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(ApplicationUserDto dto) {
            if (!ModelState.IsValid) {
                return View("CreateUser", dto);
            }
            var result = await _adminService.CreateUserAsync(dto);
            if (result.Succeeded) {
                return RedirectToAction("ShowAdminDashboardView");
            }
            foreach (var message in result.ErrorMessages) {
                ModelState.AddModelError("", message);
            }
            return View("CreateUser", dto);
        }

        [HttpPost]
        public async Task<IActionResult> CloseUserAsync(string userId) {
            var success = await _adminService.CloseUserAsync(userId);
            if (!success) {
                return NotFound();
            }
            return RedirectToAction("ShowAdminDashboard");
        }

        [HttpGet("/CreateProcessingPause")]
        public IActionResult ShowCreateProcessingPauseView(string traineeId) {
            return View("CreateProcessingPause");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcessingPauseAsync(ProcessingPauseDto dto) {
            if (!ModelState.IsValid) {
                return View("CreateProcessingPause", dto);
            }
            var result = await _adminService.CreateProcessingPauseAsync(dto);
            if (!result.Succeeded) {
                foreach (var message in result.ErrorMessages)
                ModelState.AddModelError("", message);
                return View("CreateProcessingPause", dto);
            }
            return RedirectToAction("ShowAdminDashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }
    }
}