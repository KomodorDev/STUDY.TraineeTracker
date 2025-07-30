using Microsoft.AspNetCore.Identity.UI.Services;

namespace TraineeTracker.Services.Email
{
    public class EmailSenderMock : IEmailSender {
        public Task SendEmailAsync(string email, string subject, string htmlMessage) {
            return Task.CompletedTask;
        }
    }
}