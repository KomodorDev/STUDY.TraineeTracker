using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {

    [Route("Feedback")]
    public class FeedbackController : Controller {
        private readonly FeedbackService _feedbackService;

        // ------------------------------------------------------
        public FeedbackController(FeedbackService feedbackService) {
            _feedbackService = feedbackService;
        }

        // ------------------------------------------------------
        // POST: /Feedback/Dashboard
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
        // POST: /Feedback/MarkAsRead
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
        // POST: /Feedback/MarkAsUnread
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
