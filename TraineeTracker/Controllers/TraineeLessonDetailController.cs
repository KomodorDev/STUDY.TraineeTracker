// class by schleale

using Microsoft.AspNetCore.Mvc;

using TraineeTracker.Services;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace TraineeTracker.Controllers {
    [ApiController]
    [Route("TraineeLessonDetail")]
    public class TraineeLessonDetailController : Controller {
        private readonly TraineeLessonDetailService _traineeLessonDetailService;

        public TraineeLessonDetailController(TraineeLessonDetailService traineeLessonDetailService) {
            _traineeLessonDetailService = traineeLessonDetailService;
        }
        
        [HttpGet("{traineeLessonId}")]
        public async Task<IActionResult> ShowTraineeLessonDetailView(int traineeLessonId) {
            TraineeLessonDetailViewModel viewModel = await _traineeLessonDetailService.BuildTraineeLessonDetailViewModel(traineeLessonId, User);

            return PartialView("~/Views/TraineeLessonDashboard/_TraineeLessonDetailModal.cshtml", viewModel);
        }

        // [FromBody] from json [FromFrom] from html form, either necessary for more complex types
        [HttpPost("state-change")]
        public async Task<IActionResult> SaveTraineeLessonStateChange([FromForm] TraineeLessonDto traineeLessonUpdate) {
            await _traineeLessonDetailService.SaveTraineeLessonStateChange(traineeLessonUpdate, User);

            return await ShowTraineeLessonDetailView(traineeLessonUpdate.TraineeLessonId);
        }

        // [FromBody] from json [FromFrom] from html form, either necessary for more complex types
        [HttpPost("save-feedback")]
        public async Task<IActionResult> SaveFeedback([FromForm] FeedbackDto feedback) {
            await _traineeLessonDetailService.SaveFeedback(feedback, User);

            return await ShowTraineeLessonDetailView(feedback.TraineeLessonId);
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpDelete("delete-feedback")]
        public async Task<IActionResult> DeleteFeedback(int feedbackId, int traineeLessonIdForReturn) {
            await _traineeLessonDetailService.DeleteFeedback(User, feedbackId);

            return await ShowTraineeLessonDetailView(traineeLessonIdForReturn);
        }
    }
}
