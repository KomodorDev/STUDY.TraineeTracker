using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly AdminService _adminService;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;
        private readonly ITeachingPlanRepository _teachingPlanRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminService adminService, IApplicationUserRepository applicationUserRepository, IProcessingPauseRepository processingPauseRepository, ITeachingPlanRepository teachingPlanRepository, ILogger<AdminController> logger) {
            _adminService = adminService;
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;

            _teachingPlanRepository = teachingPlanRepository;
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
        public async Task<IActionResult> CreateUserAsync(ApplicationUserDto dto) {
            var plans = await _teachingPlanRepository.GetAllTeachingPlansAsync();
            ViewBag.TeachingPlans = new SelectList(plans, "TeachingPlanId", "Name");

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
            if (user == null) {
                return NotFound();
            }
            ViewBag.Pauses = await pausesTask;
            return View("ManageProcessingPauses", user);
        }

        [HttpGet("/CreateProcessingPause")]
        public async Task<IActionResult> ShowCreateProcessingPauseView(string traineeId) {
            ViewBag.User = await _applicationUserRepository.FindByIdAsync(traineeId);
            if (ViewBag.User == null) {
                return NotFound();
            }
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
            return RedirectToAction("ShowManageProcessingPausesView", new { traineeId = dto.TraineeId });
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