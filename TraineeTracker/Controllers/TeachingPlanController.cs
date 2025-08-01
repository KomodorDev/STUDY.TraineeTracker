using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Services;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Controllers {

    // ------------------------------------------------------
    /// <summary>
    /// Controller responsible for managing the import, update, preview, and deletion of teaching plans.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask, Simon Hinterreiter
    /// </remarks>
    [Route("TeachingPlan")]
    public class TeachingPlanController : Controller {
        
        // ------------------------------------------------------
        /// <summary>
        /// Service encapsulating business logic for teaching plan operations.
        /// </summary>
        private readonly TeachingPlanService _teachingPlanService;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="TeachingPlanController"/> class.
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </summary>
        /// <param name="teachingPlanService">Service for managing teaching plan import, update, preview, and deletion.</param>
        public TeachingPlanController(TeachingPlanService teachingPlanService) {
            _teachingPlanService = teachingPlanService;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Displays the import dashboard containing existing teaching plans.
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </summary>
        /// <returns>
        /// A task that returns the "ImportDashboard" view populated with existing teaching plans.
        /// </returns>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet("Dashboard")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowImportDashboardView() {
            var existingTeachingPlans = await _teachingPlanService.BuildImportDashboardViewModelAsync();
            return View("ImportDashboard", existingTeachingPlans);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Imports a new teaching plan based on the provided DTO and redirects back to the dashboard.
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </summary>
        /// <param name="dto">The DTO containing the new teaching plan data and file.</param>
        /// <returns>
        /// A task that redirects to the import dashboard. On error, sets TempData["ImportError"] and redirects.
        /// </returns>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("ImportNewTeachingPlan")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportNewTeachingPlan(TeachingPlanDto dto) {

            /*             
            if (!ModelState.IsValid) {
                // Fehler, redirect zurück (evtl. mit TempData-Meldung)
                return RedirectToAction(nameof(ShowImportDashboardView));
            }
            */

            try {
                Console.WriteLine($"[DEBUG] Controller: Called ImportTeachingPlan");
                await _teachingPlanService.ImportNewTeachingPlan(dto);
                return RedirectToAction(nameof(ShowImportDashboardView));
            }
            catch (Exception dex) {
                // Business-Fehler anzeigen
                TempData["ImportError"] = dex.Message;
                return RedirectToAction(nameof(ShowImportDashboardView));
            }
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing teaching plan based on the provided DTO and redirects back to the dashboard.
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </summary>
        /// <param name="dto">The DTO containing updated teaching plan data and temp file name.</param>
        /// <returns>A task that redirects to the import dashboard.</returns>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("UpdateTeachingPlan")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTeachingPlan(TeachingPlanDto dto) {

            Console.WriteLine($"[DEBUG] Controller: Called UpdateTeachingPlan");
            Console.WriteLine($"[DEBUG] Controller: TeachingPlanId = {dto.ExistingTeachingPlanId}");
            Console.WriteLine($"[DEBUG] Controller: TempFileName = {dto.TempFileName}");

            await _teachingPlanService.UpdateTeachingPlan(dto);
            return RedirectToAction(nameof(ShowImportDashboardView));
        }

        // ------------------------------------------------------
        /// <summary>
        /// Loads the import preview modal for an existing teaching plan.
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </summary>
        /// <param name="planId">The ID of the teaching plan to preview.</param>
        /// <returns>
        /// A task that returns a partial view "_ImportPreviewModal" with preview data.
        /// </returns>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet("Preview/{planId}")]
        public async Task<IActionResult> LoadPreviewModal(int planId) {
            Console.WriteLine($"[DEBUG] Controller: Called LoadPreviewModal");
            Console.WriteLine($"[DEBUG] Received TeachingPlanId: {planId}");
            var dto = new TeachingPlanDto {
                ExistingTeachingPlanId = planId
            };

            var viewModel = await _teachingPlanService.BuildImportPreviewViewModelAsync(dto);
            return PartialView("_ImportPreviewModal", viewModel);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates the import preview modal based on the provided DTO.
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </summary>
        /// <param name="dto">The DTO containing existing teaching plan ID and temp file name.</param>
        /// <returns>
        /// A task that returns a partial view "_ImportPreviewModal" with updated preview data.
        /// </returns>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("Preview")]
        public async Task<IActionResult> UpdatePreviewModal(TeachingPlanDto dto) {
            Console.WriteLine($"[DEBUG] Controller: Called UpdatePreviewModal");
            Console.WriteLine($"[DEBUG] Controller: Received TeachingPlanId: {dto.ExistingTeachingPlanId}");

            var viewModel = await _teachingPlanService.BuildImportPreviewViewModelAsync(dto);
            Console.WriteLine($"[DEBUG] TeachingPlanController - UpdatePreviewModel: Built viewModel");

            Console.WriteLine($"[DEBUG] NewActiveLessons: {viewModel.NewActiveLessons.Count}");
            Console.WriteLine($"[DEBUG] NewInactiveLessons: {viewModel.NewInactiveLessons.Count}");
            Console.WriteLine($"[DEBUG] ReactivatedLessons: {viewModel.ExistingReactivatedLessons.Count}");
            Console.WriteLine($"[DEBUG] DeactivatedLessons: {viewModel.ExistingDeactivatedLessons.Count}");

            return PartialView("_ImportPreviewModal", viewModel);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a teaching plan by its ID and redirects back to the dashboard.
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </summary>
        /// <param name="existingTeachingPlanId">The ID of the teaching plan to delete.</param>
        /// <returns>
        /// A task that redirects to the import dashboard. On error, sets TempData["ImportError"] and redirects.
        /// </returns>
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("DeleteTeachingPlan")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeachingPlan(int existingTeachingPlanId) {
            try {
                await _teachingPlanService.DeleteTeachingPlan(existingTeachingPlanId);
                return RedirectToAction(nameof(ShowImportDashboardView));
            }
            catch (Exception dex) {
                // Business-Fehler anzeigen
                TempData["ImportError"] = dex.Message;
                return RedirectToAction(nameof(ShowImportDashboardView));
            }

        }

        // ------------------------------------------------------
    }
}
