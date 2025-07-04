using Microsoft.AspNetCore.Mvc;
using TraineeTracker.Services;
using TraineeTracker.Models.Domain;
using TraineeTracker.Data.TeachingPlans;

namespace TraineeTracker.Controllers
{
    public class TeachingPlanController : Controller
    {
        private readonly TeachingPlanService _teachingPlanService;

        public TeachingPlanController(TeachingPlanService teachingPlanService)
        {
            _teachingPlanService = teachingPlanService;
        }

        [Route("Import")]
        [HttpGet]
        public async Task<IActionResult> ShowTeachingplanDashboardView()
        {
            var teachingPlans = await _teachingPlanService.GetAllTeachingPlansAsync();
            return View("ImportDashboard", teachingPlans);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportNewTeachingPlan(IFormFile file, string name)
        {
            try
            {
                await _teachingPlanService.ImportNewTeachingPlan(file, name);
                return RedirectToAction("ShowTeachingplanDashboardView");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Error");
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateTeachingPlan(IFormFile file, int teachingPlanId)
        {
            try
            {
                await _teachingPlanService.UpdateTeachingPlan(file, teachingPlanId);
                return RedirectToAction("ShowTeachingplanDashboardView");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Error");
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteTeachingPlan(int teachingPlanId)
        {
            try
            {
                await _teachingPlanService.DeleteTeachingPlan(teachingPlanId);
                return RedirectToAction("ShowTeachingplanDashboardView");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Error");
            }
        }
    }
}
