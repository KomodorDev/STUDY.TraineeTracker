// class by schleale

using Microsoft.AspNetCore.Mvc;

using TraineeTracker.Services;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeLessonDetailController : ControllerBase {
        private readonly TraineeLessonDetailService _traineeLessonDetailService;

        public TraineeLessonDetailController(TraineeLessonDetailService traineeLessonDetailService) {
            _traineeLessonDetailService = traineeLessonDetailService;
        }
        
        [HttpGet("{traineeLessonId}")]
        public async Task<IActionResult> ShowTraineeLessonDetailView(int traineeLessonId) {
            var viewModel = await _traineeLessonDetailService.BuildTraineeLessonDetailViewModel(traineeLessonId, User);

            return Ok(viewModel);
        }

        // [FromBody] : "deserialize the JSON in the request body into this C# object", necessary for more complex types
        [HttpPost("state-change")]
        public async Task<IActionResult> SaveTraineeLessonStateChange([FromBody] TraineeLessonDto traineeLessonUpdate) {
            await _traineeLessonDetailService.SaveTraineeLessonStateChange(traineeLessonUpdate, User);

            return NoContent();
        }

        // [FromBody] : "deserialize the JSON in the request body into this C# object", necessary for more complex types
        [HttpPost("save-feedback")]
        public async Task<IActionResult> SaveFeedback([FromBody] FeedbackDto feedback) {
            await _traineeLessonDetailService.SaveFeedback(feedback, User);

            return NoContent();
        }

        [HttpDelete("delete-feedback")]
        public IActionResult DeleteFeedback(int feedbackId) {
            _traineeLessonDetailService.DeleteFeedback(User, feedbackId);

            return NoContent();
        }
    }
}
