using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using TraineeTracker.Services;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;

namespace TraineeTracker.Controllers {

    /// <summary>
    /// Controller to manage trainee lesson detail modal, and hence feedback creation / modification, and state changes.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    [ApiController]
    [Route("TraineeLessonDetail")]
    public class TraineeLessonDetailController : Controller {

        /// <summary>
        /// Service to execute modification requests for trainee lessons, e.g. feedbacks and states.
        /// </summary>
        private readonly TraineeLessonDetailService _traineeLessonDetailService;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        /// <summary>
        /// Constructor to initialize TraineeLessonDetailService dependency.
        /// </summary>
        /// <param name="traineeLessonDetailService">Service for trainee lesson detail operations</param>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public TraineeLessonDetailController(TraineeLessonDetailService traineeLessonDetailService) {
            _traineeLessonDetailService = traineeLessonDetailService;
        }
        
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        /// <summary>
        /// Retrieves the detailed view modal content for a specific trainee lesson.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the requested trainee lesson</param>
        /// <returns>Partial view with the trainee lesson detail model</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        [HttpGet("{traineeLessonId}")]
        public async Task<IActionResult> ShowTraineeLessonDetailView(int traineeLessonId) {
            TraineeLessonDetailViewModel viewModel = await _traineeLessonDetailService.BuildTraineeLessonDetailViewModel(traineeLessonId, User);

            return PartialView("~/Views/TraineeLessonDashboard/_TraineeLessonDetailModal.cshtml", viewModel);
        }

        // ----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Saves state changes for a trainee lesson.
        /// </summary>
        /// <param name="traineeLessonUpdate">DTO containing updated trainee lesson data</param>
        /// <returns>Updated trainee lesson detail view</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        // [FromBody] from json [FromFrom] from html form, either necessary for more complex types
        [HttpPost("state-change")]
        public async Task<IActionResult> SaveTraineeLessonStateChange([FromForm] TraineeLessonDto traineeLessonUpdate) {
            await _traineeLessonDetailService.SaveTraineeLessonStateChange(traineeLessonUpdate, User);

            return await ShowTraineeLessonDetailView(traineeLessonUpdate.TraineeLessonId);
        }

        // ----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Saves new or updated feedback for a trainee lesson. Necessary for transitioning to state Rated.
        /// </summary>
        /// <param name="feedback">DTO containing feedback data</param>
        /// <returns>Updated trainee lesson detail view</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        // [FromBody] from json [FromFrom] from html form, either necessary for more complex types
        [HttpPost("save-feedback")]
        public async Task<IActionResult> SaveFeedback([FromForm] FeedbackDto feedback) {
            await _traineeLessonDetailService.SaveFeedback(feedback, User);

            return await ShowTraineeLessonDetailView(feedback.TraineeLessonId);
        }

        // ----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes feedback from a trainee lesson. Only Mentors and Admins are authorized to do so.
        /// </summary>
        /// <param name="feedbackId">ID of the feedback to delete</param>
        /// <param name="traineeLessonIdForReturn">Trainee lesson ID to return to after deletion</param>
        /// <returns>Updated trainee lesson detail view</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        [HttpPost("delete-feedback")]
        public async Task<IActionResult> DeleteFeedback([FromForm] int feedbackId, [FromForm] int traineeLessonIdForReturn) {
            await _traineeLessonDetailService.DeleteFeedback(User, feedbackId);

            return await ShowTraineeLessonDetailView(traineeLessonIdForReturn);
        }
    }
}
