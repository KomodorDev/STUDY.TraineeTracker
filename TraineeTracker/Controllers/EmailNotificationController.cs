using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Email;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class EmailNotificationController : Controller {
    private readonly EmailNotificationService _emailNotificationService;


    // ------------------------------------------------------
    public EmailNotificationController(EmailNotificationService emailNotificationService) {
        _emailNotificationService = emailNotificationService;
    }


    // ------------------------------------------------------
    // http://localhost:5079/NotificationSettings
    [Route("Notifications")]
    [HttpGet]
    public async Task<IActionResult> ShowEmailNotificationDashboardView() {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var setting = await _emailNotificationService.GetNotificationSetting(userId);
        return View("EmailNotificationSettings", setting);
    }

    // ------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> SaveEmailNotificationSettingChange(EmailNotificationSettingDto update) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        await _emailNotificationService.SaveNotificationSettingChange(userId, update);
        return RedirectToAction(nameof(ShowEmailNotificationDashboardView));
    }

    // ------------------------------------------------------
}
