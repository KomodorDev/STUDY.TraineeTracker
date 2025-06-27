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
        public IActionResult ShowTraineeStatisticsDashboardView()
        {
            var traineeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (traineeId == null)
            {
                return Unauthorized();
            }

            var model = _service.BuildTraineeStatisticsViewModel(traineeId);
            return View(model);
        }
    }
}