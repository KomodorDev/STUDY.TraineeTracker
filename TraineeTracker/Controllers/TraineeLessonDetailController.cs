// class by schleale

using Microsoft.AspNetCore.Mvc;

using TraineeTracker.Services;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.Domain;
using System.Threading.Tasks;
using TraineeTracker.Models.ViewModels;

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

        // [FromBody] : "deserialize the JSON in the request body into this C# object", necessary for more complex types
        [HttpPost("state-change")]
        public async Task<IActionResult> SaveTraineeLessonStateChange([FromBody] TraineeLessonDto traineeLessonUpdate) {
            await _traineeLessonDetailService.SaveTraineeLessonStateChange(traineeLessonUpdate, User);

            // reloads page
            return RedirectToAction("ShowTraineeLessonDetailView", "TraineeLessonDetailController", new { traineeLessonId = traineeLessonUpdate.TraineeLessonId });
        }

        // [FromBody] : "deserialize the JSON in the request body into this C# object", necessary for more complex types
        [HttpPost("save-feedback")]
        public async Task<IActionResult> SaveFeedback([FromBody] FeedbackDto feedback) {
            await _traineeLessonDetailService.SaveFeedback(feedback, User);

            return RedirectToAction("ShowTraineeLessonDetailView", "TraineeLessonDetailController", new { traineeLessonId = feedback.TraineeLessonId });
        }

        [HttpDelete("delete-feedback")]
        public async Task<IActionResult> DeleteFeedback(int feedbackId, int traineeLessonIdForReturn) {
            await _traineeLessonDetailService.DeleteFeedback(User, feedbackId);

            return RedirectToAction("ShowTraineeLessonDetailView", new { traineeLessonIdForReturn });
        }
    }
}
