using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {
    [Authorize]
    public class FeedbackController : Controller {
        private readonly FeedbackService _feedbackService;

        // ------------------------------------------------------
        public FeedbackController(FeedbackService feedbackService) {
            _feedbackService = feedbackService;
        }

        // ------------------------------------------------------
        // GET: /Feedback/Dashboard
        [Route("FeedbackDashboard")]
        [HttpGet]
        public async Task<IActionResult> ShowFeedbackDashboardView() {
            var viewModel = await _feedbackService.BuildFeedbackDashboardViewModelAsync(User);
            return View("FeedbackDashboard", viewModel);
        }

        // ------------------------------------------------------
        // GET: /Feedback/All?page=1
        public async Task<IActionResult> All(int page = 1) {
            var allPage = await _feedbackService.GetAllFeedbacksAsync(page);
            return View(allPage);
        }

        // ------------------------------------------------------
        // GET: /Feedback/Unread?page=1
        public async Task<IActionResult> Unread(int page = 1) {
            var unreadPage = await _feedbackService.GetUnreadFeedbacksAsync(User, page);
            return View(unreadPage);
        }
        // ------------------------------------------------------
        // GET: /Feedback/Read?page=1
        public async Task<IActionResult> Read(int page = 1) {
            var readPage = await _feedbackService.GetReadFeedbacksAsync(User, page);
            return View(readPage);
        }

        // ------------------------------------------------------
        // POST: /Feedback/MarkAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int feedbackId, int page = 1) {
            await _feedbackService.MarkFeedbackAsReadAsync(User, feedbackId);

            // 1) Referer-Header auslesen
            var referer = Request.Headers["Referer"].ToString();

            // 2) Nur lokale URLs zulassen
            if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(referer))
                return Redirect(referer);

            // Fallback: Dashboard
            return RedirectToAction(nameof(ShowFeedbackDashboardView));
        }

        // ------------------------------------------------------

    }
}
