using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Email;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Controller responsible for displaying and updating a user's email notification settings.
/// </summary>
/// <remarks>
/// Code Ownership: Simon Hinterreiter (hintsimo)
/// </remarks>
[Authorize]
public class EmailNotificationController : Controller {

    /// <summary>
    /// Service responsible for retrieving and updating email notification settings.
    /// </summary>
    private readonly EmailNotificationService _emailNotificationService;

    // ------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailNotificationController"/> class.
    /// </summary>
    /// <param name="emailNotificationService">Service for handling email notification logic.</param>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public EmailNotificationController(EmailNotificationService emailNotificationService) {
        _emailNotificationService = emailNotificationService;
    }

    // ------------------------------------------------------
    /// <summary>
    /// Displays the email notification settings view for the currently authenticated user.
    /// </summary>
    /// <returns>
    /// A Task representing the asynchronous operation,
    /// which returns a view showing the user's current notification settings.
    /// </returns>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    [HttpGet("Notifications")]
    public async Task<IActionResult> ShowEmailNotificationDashboardView() {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var setting = await _emailNotificationService.GetNotificationSetting(userId);
        return View("EmailNotificationSettings", setting);
    }

    // ------------------------------------------------------
    /// <summary>
    /// Updates the email notification settings for the currently authenticated user
    /// based on the provided <see cref="NotificationSettingDto"/>.
    /// </summary>
    /// <param name="update">The updated notification settings.</param>
    /// <returns>
    /// A Task representing the asynchronous operation,
    /// which redirects to the notification dashboard view.
    /// </returns>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
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
