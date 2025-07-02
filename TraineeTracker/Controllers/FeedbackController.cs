using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {
    [Authorize]
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

            // 1) Referer-Header auslesen
            var referer = Request.Headers["Referer"].ToString();

            // 2) Nur lokale URLs zulassen
            if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(referer))
                return Redirect(referer);

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

            var referer = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(referer))
                return Redirect(referer);

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
