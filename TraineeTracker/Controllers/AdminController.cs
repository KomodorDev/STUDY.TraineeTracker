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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View("Error!");
        }
    }
}