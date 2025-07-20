// class by schleale

using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Services;
using Microsoft.AspNetCore.Authorization;

namespace TraineeTracker.Controllers {
    [Authorize]
    public class TraineeLessonDashboardController : Controller {
        private readonly TraineeLessonDashboardService _traineeLessonDashboardService;

        public TraineeLessonDashboardController(TraineeLessonDashboardService traineeLessonDashboardService) {
            _traineeLessonDashboardService = traineeLessonDashboardService;
        }

        // ------------------------------------------------------

        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> ShowTraineeLessonDashboardView(string? traineeId,
            string filter = "all",
            string sortBy = "SortingIndex_asc") {
            var model = await _traineeLessonDashboardService.BuildTraineeLessonDashboardViewModel(User, traineeId, filter, sortBy);

            return View("TraineeLessonDashboard", model);
        }
    }
}