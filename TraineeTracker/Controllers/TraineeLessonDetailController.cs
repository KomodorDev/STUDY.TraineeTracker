using Microsoft.AspNetCore.Mvc;

using TraineeTracker.Services;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeLessonDetailController : ControllerBase {

        [HttpGet("{traineeLessonId}")]
        public async Task<IActionResult> ShowTraineeLessonDetailView(int traineeLessonId) {
            await TraineeLessonDetailService.BuildTraineeLessonDetailViewModel();

            return
        }

        // [FromBody] : "deserialize the JSON in the request body into this C# object", necessary for more complex types
        [HttpPost("state-change")]
        public IActionResult SaveTraineeLessonStateChange([FromBody] TraineeLessonDto traineeLessonUpdate) {
            TraineeLessonDetailService.SaveTraineeLessonStateChange(traineeLessonUpdate, User);

            return
        }

        // [FromBody] : "deserialize the JSON in the request body into this C# object", necessary for more complex types
        [HttpPost("save-feedback")]
        public IActionResult SaveFeedback([FromBody] FeedbackDto feedback) {
            TraineeLessonDetailController.SaveFeedback(feedback, User, );

            return
        }

        [HttpPost("delete-feedback")]
        public IActionResult DeleteFeedback(int feedbackId) {
            TraineeLessonDetailController.DeleteFeedback(feedbackId);
            return
        }
    }
}