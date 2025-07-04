using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Services;

namespace TraineeTracker.Controllers {
    public class TeachingPlanController : Controller {
        private readonly TeachingPlanService _teachingPlanService;


        // ------------------------------------------------------
        public TeachingPlanController(TeachingPlanService teachingPlanService) {
            _teachingPlanService = teachingPlanService;
        }


        // ------------------------------------------------------
        [HttpGet]
        [Route("ImportDashboard")]
        public async Task<IActionResult> ImportDashboard() {
            var vm = await _teachingPlanService.BuildImportDashboardAsync();
            return View("ImportDashboard", vm);
        }

        // ------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportNewTeachingPlan(ImportDashboardViewModel model) {
            if (!ModelState.IsValid) {
                // Bei Validierungsfehlern die Liste neu laden und zurück zur View
                var vm = await _teachingPlanService.BuildImportDashboardAsync();
                vm.NewPlanName = model.NewPlanName;
                return View("ImportDashboard", vm);
            }

            try {
                await _teachingPlanService.ImportNewTeachingPlan(model.NewPlanFile, model.NewPlanName);
                return RedirectToAction(nameof(ImportDashboard));
            }
            catch (Exception dex) {
                // nur die Business-Fehler hier behandeln
                ModelState.AddModelError(nameof(model.NewPlanName), dex.Message);
                var vm = await _teachingPlanService.BuildImportDashboardAsync();
                vm.NewPlanName = model.NewPlanName;
                return View("ImportDashboard", vm);
            }
        }

        // ------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTeachingPlan(int teachingPlanId, IFormFile file) {
            if (file == null) {
                ModelState.AddModelError(nameof(file), "Bitte eine Datei auswählen");
                var vm = await _teachingPlanService.BuildImportDashboardAsync();
                return View("ImportDashboard", vm);
            }

            await _teachingPlanService.UpdateTeachingPlan(file, teachingPlanId);
            return RedirectToAction(nameof(ImportDashboard));
        }


        // ------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeachingPlan(int teachingPlanId) {
            await _teachingPlanService.DeleteTeachingPlan(teachingPlanId);
            return RedirectToAction(nameof(ImportDashboard));
        }
    }
}
