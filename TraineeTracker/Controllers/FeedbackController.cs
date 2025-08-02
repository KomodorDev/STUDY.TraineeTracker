using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {

    // ------------------------------------------------------
    /// <summary>
    /// Controller responsible for displaying the feedback dashboard
    /// and handling actions to mark feedback as read or unread.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask, Simon Hinterreiter
    /// </remarks>
    [Route("Feedback")]
    public class FeedbackController : Controller {

        /// <summary>
        /// Service encapsulating business logic for feedback operations.
        /// </summary>
        private readonly FeedbackService _feedbackService;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackController"/> class.
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </summary>
        /// <param name="feedbackService">Service for managing feedback dashboard and read/unread actions.</param>
        /// <remarks>
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </remarks>
        public FeedbackController(FeedbackService feedbackService) {
            _feedbackService = feedbackService;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Displays the feedback dashboard with optional filter, pagination, and sorting.
        /// </summary>
        /// <param name="filter">Filter type ("all", "read", or "unread").</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="sortBy">Sort criteria, e.g. "date_asc".</param>
        /// <param name="selectedTraineeId">Optional trainee ID to filter by.</param>
        /// <param name="selectedLessonId">Optional lesson ID to filter by.</param>
        /// <param name="selectedTeachingPlanId">Optional teaching plan ID to filter by.</param>
        /// <returns>
        /// A task representing the asynchronous operation. Returns the "FeedbackDashboard" view populated with a view model.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </remarks>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet("Dashboard")]
        public async Task<IActionResult> ShowFeedbackDashboardView(
                    string filter = "all",
                    int page = 1,
                    string sortBy = "date_asc",
                    string? selectedTraineeId = null,
                    int? selectedLessonId = null,
                    int? selectedTeachingPlanId = null) {

            // +++++++++++++++
            // Build ViewModel
            var viewModel = await _feedbackService.BuildFeedbackDashboardViewModelAsync(
                User, filter, page, sortBy, selectedTraineeId, selectedLessonId, selectedTeachingPlanId);

            // +++++++++++++++
            // Return View
            return View("FeedbackDashboard", viewModel);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Marks a specific feedback entry as read for the current user and redirects back to the dashboard.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to mark as read.</param>
        /// <param name="filter">Current filter setting to preserve state.</param>
        /// <param name="page">Current page number to preserve state.</param>
        /// <param name="sortBy">Current sort criteria to preserve state.</param>
        /// <param name="selectedTraineeId">Current trainee filter to preserve state.</param>
        /// <param name="selectedLessonId">Current lesson filter to preserve state.</param>
        /// <param name="selectedTeachingPlanId">Current teaching plan filter to preserve state.</param>
        /// <returns>
        /// A task representing the asynchronous operation. Redirects to the Dashboard action with preserved route values.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </remarks>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(
            int feedbackId,
            string filter = "all",
            int page = 1,
            string sortBy = "date_asc",
            string? selectedTraineeId = null,
            int? selectedLessonId = null,
            int? selectedTeachingPlanId = null) {

            // +++++++++++++++
            // Mark Feedback As Read
            await _feedbackService.MarkFeedbackAsReadAsync(User, feedbackId);

            // +++++++++++++++
            // Return to last View
            return RedirectToAction(
                actionName: "Dashboard",
                controllerName: "Feedback",
                routeValues: new {
                    filter,
                    page,
                    sortBy,
                    selectedTraineeId,
                    selectedLessonId,
                    selectedTeachingPlanId
                });
        }

        // ------------------------------------------------------
        /// <summary>
        /// Marks a specific feedback entry as unread for the current user and redirects back to the dashboard.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to mark as unread.</param>
        /// <param name="filter">Current filter setting to preserve state.</param>
        /// <param name="page">Current page number to preserve state.</param>
        /// <param name="sortBy">Current sort criteria to preserve state.</param>
        /// <param name="ascending">Flag indicating ascending sort order (unused but preserved).</param>
        /// <param name="selectedTraineeId">Current trainee filter to preserve state.</param>
        /// <param name="selectedLessonId">Current lesson filter to preserve state.</param>
        /// <param name="selectedTeachingPlanId">Current teaching plan filter to preserve state.</param>
        /// <returns>
        /// A task representing the asynchronous operation. Redirects to the Dashboard action with preserved route values.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </remarks>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("MarkAsUnread")]
        public async Task<IActionResult> MarkAsUnread(
            int feedbackId,
            string filter = "all",
            int page = 1,
            string sortBy = "date_asc",
            bool ascending = false,
            string? selectedTraineeId = null,
            int? selectedLessonId = null,
            int? selectedTeachingPlanId = null) {

            // +++++++++++++++
            // Mark Feedback As Unread
            await _feedbackService.MarkFeedbackAsUnreadAsync(User, feedbackId);

            // +++++++++++++++
            // Return to last View
            return RedirectToAction(
                actionName: "Dashboard",
                controllerName: "Feedback",
                routeValues: new {
                    filter,
                    page,
                    sortBy,
                    selectedTraineeId,
                    selectedLessonId,
                    selectedTeachingPlanId
                });
        }

        // ------------------------------------------------------

    }
}
