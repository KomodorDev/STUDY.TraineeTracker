using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {
    public class TraineeStatisticsController : Controller {
        private readonly TraineeStatisticsService _service;
        public TraineeStatisticsController(TraineeStatisticsService service) {
            _service = service;
        }

        [HttpGet("TraineeStatistics/{traineeId}")]
        public async Task<IActionResult> ShowTraineeStatisticsDashboardView(string traineeId) {
            
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null) {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");
            var isMentor = User.IsInRole("Mentor");
            var isTraineeSelf = currentUserId == traineeId;

            if (!(isAdmin || isMentor || isTraineeSelf)) {
                return Forbid();
            }

            var model = await _service.BuildTraineeStatisticsViewModel(traineeId, User);
            return PartialView("_TraineeStatistics", model);
        }
    }
}