using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Services;
using Microsoft.AspNetCore.Authorization;

namespace TraineeTracker.Controllers {

    /// <summary>
    /// Handles requests related to the trainee lesson dashboard view.
    /// Requires the user to be authenticated.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    [Authorize]
    public class TraineeLessonDashboardController : Controller {

        /// <summary>
        /// Service for building the trainee lesson dashboard view model.
        /// </summary>
        private readonly TraineeLessonDashboardService _traineeLessonDashboardService;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="TraineeLessonDashboardController"/> class.
        /// </summary>
        /// <param name="traineeLessonDashboardService">The service used to build the dashboard view model.</param>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public TraineeLessonDashboardController(TraineeLessonDashboardService traineeLessonDashboardService) {
            _traineeLessonDashboardService = traineeLessonDashboardService;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Displays the trainee lesson dashboard view for the authenticated user.
        /// </summary>
        /// <param name="traineeId">Optional trainee ID to filter dashboard data. If null, defaults to current user.</param>
        /// <param name="filter">A string representing the filter to apply (e.g., "all", "completed").</param>
        /// <param name="sortBy">A string representing the sort order (e.g., "SortingIndex_asc").</param>
        /// <returns>An <see cref="IActionResult"/> that renders the trainee lesson dashboard view.</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> ShowTraineeLessonDashboardView(string? traineeId,
            string filter = "all",
            string sortBy = "state_custom") {
            var model = await _traineeLessonDashboardService.BuildTraineeLessonDashboardViewModel(User, traineeId, filter, sortBy);

            return View("TraineeLessonDashboard", model);
        }

        // ------------------------------------------------------
    }
}