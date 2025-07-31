using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {

    /// <summary>
    /// Handles requests related to the trainee statistics view.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Nikita Stefan (stefanni)
    /// </remarks>
    public class TraineeStatisticsController : Controller {

        /// <summary>
        /// Service used for building the trainee statistics view model.
        /// </summary>
        private readonly TraineeStatisticsService _service;

        // ------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="TraineeStatisticsController"/> class with the provided service.
        /// </summary>
        /// <param name="service">The service responsible for building trainee statistics.</param>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        public TraineeStatisticsController(TraineeStatisticsService service) {
            _service = service;
        }

        // ------------------------------------------------------

        /// <summary>
        /// Displays the trainee statistics partial view for a given trainee.
        /// Access is restricted to the trainee themselves, mentors, or admins.
        /// </summary>
        /// <param name="traineeId">The ID of the trainee whose statistics should be shown.</param>
        /// <returns>
        /// A partial view containing the statistics, or an appropriate authorization result.
        /// </returns>
        /// <response code="401">If the user is not logged in.</response>
        /// <response code="403">If the user is not authorized to view the requested trainee's statistics.</response>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        [HttpGet("TraineeStatistics/{traineeId}")]
        public async Task<IActionResult> ShowTraineeStatisticsDashboardView(string traineeId) {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null) {
                return Unauthorized();
            }

            try {
                var model = await _service.BuildTraineeStatisticsViewModel(traineeId, User);
                return PartialView("_TraineeStatistics", model);
            }
            catch (UnauthorizedAccessException) {
                return Forbid();
            }
        }

        // ------------------------------------------------------

    }
}
