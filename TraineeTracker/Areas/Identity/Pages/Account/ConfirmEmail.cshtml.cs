// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Areas.Identity.Pages.Account {
    public class ConfirmEmailModel : PageModel {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Handles GET requests for the email confirmation page.
        /// Validates the confirmation token and updates the user's email status.
        /// On success, redirects the user to the password reset page.
        /// On failure, displays an error message.
        /// </summary>
        /// <param name="userId">The ID of the user whose email is being confirmed.</param>
        /// <param name="code">The confirmation token sent to the user's email.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that redirects to the password reset page on success,
        /// or displays an error message on failure.
        /// </returns>
        public async Task<IActionResult> OnGetAsync(string userId, string code) {
            if (userId == null || code == null) {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded) {
                StatusMessage = "Thank you for confirming your email. You will be redirected to set your password.";
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                resetToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
                return RedirectToPage("/Account/ResetPassword", new { userId = user.Id, code = resetToken });
            } else {
                StatusMessage = $"Error confirming your email: {result.Errors.First().Description}";
            }
            return Page();
        }
    }
}
