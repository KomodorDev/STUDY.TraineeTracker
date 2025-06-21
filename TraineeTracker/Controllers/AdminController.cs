using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Controllers {
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class AdminController : Controller {
        private readonly AdminService _adminService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminService adminService, IApplicationUserRepository applicationUserRepository, ILogger<AdminController> logger) {
            _adminService = adminService;
            _applicationUserRepository = applicationUserRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ManageUsers() {
            return View();
        }

        [HttpGet]
        public IActionResult CreateUser() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(ApplicationUserDto dto) {
            if (!ModelState.IsValid) {
                return View(dto);
            }
            var result = await _adminService.CreateUserAsync(dto);
            if (result.Succeeded) {
                return RedirectToAction("ManageUsers");
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
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> OpenUserAsync(string userId) {
            var success = await _adminService.SetIsClosedAsync(userId, false);
            if (!success) {
                return NotFound();
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public IActionResult CreateProcessingPause() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcessingPauseAsync(string userId, ProcessingPauseDto dto) {
            if (!ModelState.IsValid) {
                return View(dto);
            }
            var result = await _adminService.CreateProcessingPauseAsync(userId, dto);
            if (!result.Succeeded) {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(dto);
            }
            return RedirectToAction("ManageUsers");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }
    }
}