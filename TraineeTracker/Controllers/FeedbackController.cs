using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Models.ViewModels;
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
        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet("Dashboard")]
        public async Task<IActionResult> ShowFeedbackDashboardView(
                    string filter = "all",
                    int page = 1,
                    string sortBy = "date",
                    bool ascending = false,
                    string? selectedTraineeId = null,
                    int? selectedLessonId = null) {
            var viewModel = await _feedbackService.BuildFeedbackDashboardViewModelAsync(
                User, filter, page, sortBy, ascending, selectedTraineeId, selectedLessonId);

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
            string sortBy = "date",
            bool ascending = false,
            string? selectedTraineeId = null,
            int? selectedLessonId = null) {

            await _feedbackService.MarkFeedbackAsReadAsync(User, feedbackId);

            // Fallback: Dashboard
            return RedirectToAction(
                actionName: "Dashboard",
                controllerName: "Feedback",
                routeValues: new {
                    filter,
                    page,
                    sortBy,
                    ascending,
                    selectedTraineeId,
                    selectedLessonId
                });
        }

        // ------------------------------------------------------
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("MarkAsUnread")]
        public async Task<IActionResult> MarkAsUnread(
            int feedbackId,
            string filter = "all",
            int page = 1,
            string sortBy = "date",
            bool ascending = false,
            string? selectedTraineeId = null,
            int? selectedLessonId = null) {

            await _feedbackService.MarkFeedbackAsUnreadAsync(User, feedbackId);

            return RedirectToAction(
                actionName: "Dashboard",
                controllerName: "Feedback",
                routeValues: new {
                    filter,
                    page,
                    sortBy,
                    ascending,
                    selectedTraineeId,
                    selectedLessonId
                });
        }

        // ------------------------------------------------------

    }
}
