using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Email;

public class EmailNotificationController : Controller {
    private readonly EmailNotificationService _emailNotificationService;


    // ------------------------------------------------------
    public EmailNotificationController(EmailNotificationService emailNotificationService) {
        _emailNotificationService = emailNotificationService;
    }


    // ------------------------------------------------------
    [HttpGet]
    public IActionResult ShowEmailNotificationDashboardView() {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var settings = _emailNotificationService.GetNotificationSetting(userId);
        return View("Dashboard", settings); // assumes Dashboard.cshtml exists
    }

    // ------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> SaveEmailNotificationSettingChange(NotificationSettingDto update) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        await _emailNotificationService.SaveNotificationSettingChange(userId, update);
        return RedirectToAction(nameof(ShowEmailNotificationDashboardView));
    }
    
     // ------------------------------------------------------
}
