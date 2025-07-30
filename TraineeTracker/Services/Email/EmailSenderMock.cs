using Microsoft.AspNetCore.Identity.UI.Services;

namespace TraineeTracker.Services.Email {

    /// <summary>
    /// A mock implementation of <see cref="IEmailSender"/> that simulates sending emails without actually sending them.
    /// Useful for testing purposes where email delivery is not required.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class EmailSenderMock : IEmailSender {
        
        /// <summary>
        /// Simulates sending an email asynchronously. This mock implementation does nothing and completes immediately.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="htmlMessage">The HTML content of the email message.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public Task SendEmailAsync(string email, string subject, string htmlMessage) {
            return Task.CompletedTask;
        }
    }

}