using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Services;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Controllers {

    [Route("TeachingPlan")]
    public class TeachingPlanController : Controller {
        private readonly TeachingPlanService _teachingPlanService;

        // ------------------------------------------------------
        public TeachingPlanController(TeachingPlanService teachingPlanService) {
            _teachingPlanService = teachingPlanService;
        }

        // ------------------------------------------------------
        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet("Dashboard")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowImportDashboardView() {
            var existingTeachingPlans = await _teachingPlanService.BuildImportDashboardViewModelAsync();
            return View("ImportDashboard", existingTeachingPlans);
        }

        // ------------------------------------------------------
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
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("UpdateTeachingPlan")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTeachingPlan(TeachingPlanDto dto) {

            Console.WriteLine($"[DEBUG] Controller: Called UpdateTeachingPlan");
            if (dto.NewPlanFile == null) {
                Console.WriteLine($"[DEBUG] Controller: NewPlanFile is null");
                ModelState.AddModelError(nameof(dto.NewPlanFile), "Bitte eine Datei auswählen");
                var existingTeachingPlans = await _teachingPlanService.BuildImportDashboardViewModelAsync();
                return View("ImportDashboard", existingTeachingPlans);
            }

            await _teachingPlanService.UpdateTeachingPlan(dto);
            return RedirectToAction(nameof(ShowImportDashboardView));
        }


        // ------------------------------------------------------
        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("DeleteTeachingPlan")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeachingPlan(int teachingPlanId) {
            await _teachingPlanService.DeleteTeachingPlan(teachingPlanId);
            return RedirectToAction(nameof(ShowImportDashboardView));
        }

        // ------------------------------------------------------
    }
}
