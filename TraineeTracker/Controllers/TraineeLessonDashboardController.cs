// class by schleale

using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {
    public class TraineeLessonDashboardController : Controller {
        private readonly TraineeLessonDashboardService _traineeLessonDashboardService;

        public TraineeLessonDashboardController(TraineeLessonDashboardService traineeLessonDashboardService) {
            _traineeLessonDashboardService = traineeLessonDashboardService;
        }

        // ------------------------------------------------------

        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> ShowTraineeLessonDashboardView(string? traineeId) {
            var model = await _traineeLessonDashboardService.BuildTraineeLessonDashboardViewModel(User, traineeId);

            return View("TraineeLessonDashboard", model);
        }
    }
}