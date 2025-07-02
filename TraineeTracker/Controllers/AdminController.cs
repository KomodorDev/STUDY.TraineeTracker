using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly AdminService _adminService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminService adminService, IApplicationUserRepository applicationUserRepository, IProcessingPauseRepository processingPauseRepository, ILogger<AdminController> logger) {
            _adminService = adminService;
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;
            _logger = logger;
        }

        [HttpGet("/ManageUsers")]
        public async Task<IActionResult> ShowAdminDashboardView() {
            var usersTask = _applicationUserRepository.GetAllAsync();
            var rolesTask = _adminService.GetUserRoles();
            await Task.WhenAll(usersTask, rolesTask);
            var users = await usersTask;
            ViewBag.UserRoles = await rolesTask;
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
            var userTask = _applicationUserRepository.FindByIdAsync(traineeId);
            var pausesTask = _processingPauseRepository.GetAllPausesAsync(traineeId);
            await Task.WhenAll(userTask, pausesTask);
            var user = await userTask;
            ViewBag.Pauses = await pausesTask;
            return View("ManageProcessingPauses", user);
        }

        [HttpGet("/CreateProcessingPause")]
        public async Task<IActionResult> ShowCreateProcessingPauseView(string traineeId) {
            ViewBag.User = await _applicationUserRepository.FindByIdAsync(traineeId);
            return View("CreateProcessingPause", new ProcessingPauseDto { TraineeId = traineeId });
        }

        [HttpPost("/CreateProcessingPause")]
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

        [HttpPost("/DeleteProcessingPause")]
        public async Task<IActionResult> DeleteProcessingPauseAsync(int processingPauseId) {
            var pause = await _processingPauseRepository.FindById(processingPauseId);
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