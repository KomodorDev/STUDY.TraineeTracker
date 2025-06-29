// class by schleale

using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeLessonDashboardController : Controller {
        private readonly TraineeLessonDashboardService _traineeLessonDashboardService;

        public TraineeLessonDashboardController(TraineeLessonDashboardService traineeLessonDashboardService) {
            _traineeLessonDashboardService = traineeLessonDashboardService;
        }

        // ------------------------------------------------------

        [HttpGet("{traineeId}")]
        public IActionResult ShowTraineeLessonDashboardView(string traineeId) {
            var model = _traineeLessonDashboardService.BuildTraineeLessonDashboardViewModel(User, traineeId);

            return View("TraineeLessonDashboard", model);
        }
    }
}