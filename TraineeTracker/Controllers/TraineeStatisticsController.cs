using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TraineeTracker.Services;
using TraineeTracker.Models.ViewModels;

namespace TraineeTracker.Controllers {
    public class TraineeStatisticsController : Controller
    {
        private readonly TraineeStatisticsService _service;
        public TraineeStatisticsController(TraineeStatisticsService service)
        {
            _service = service;
        }
        [Route("TraineeStatistics/{traineeId}")]
        [HttpGet]
        public async Task<IActionResult> ShowTraineeStatisticsDashboardView(string traineeId) {
            Console.WriteLine("View aufrufen");
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Console.WriteLine(traineeId);
            Console.WriteLine(currentUserId);

            if (currentUserId == null) {
                Console.WriteLine("UserId = null");
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");
            var isMentor = User.IsInRole("Mentor");
            var isTraineeSelf = currentUserId == traineeId;

            if (!(isAdmin || isMentor || isTraineeSelf)) {
                Console.WriteLine("falscher User");
                return Forbid();
            }
            Console.WriteLine("ViewModel bauen und View final anzeigen");

            var model = await _service.BuildTraineeStatisticsViewModel(traineeId, User);
            return PartialView("_TraineeStatistics", model);
        }
    }
}