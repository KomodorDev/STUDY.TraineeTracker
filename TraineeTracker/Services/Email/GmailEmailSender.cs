using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

public class GmailEmailSender : IEmailSender {
    private readonly EmailSettings _settings;

    public GmailEmailSender(IOptions<EmailSettings> settings) {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage) {
        var fromEmail = _settings.From;
        var appPassword = _settings.Password;

        var message = new MailMessage {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(email);

        using var smtp = new SmtpClient("smtp.gmail.com", 587) {
            Credentials = new NetworkCredential(fromEmail, appPassword),
            EnableSsl = true,
            // DeliveryMethod = SmtpDeliveryMethod.Network,
            // UseDefaultCredentials = false // <-- this is critical
        };

        try {
            await smtp.SendMailAsync(message);
        }
        catch (SmtpException ex) {
            Console.WriteLine("SMTP error:");
            Console.WriteLine(ex.StatusCode);  // May be GeneralFailure, ClientNotPermitted, etc.
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (Exception ex) {
            Console.WriteLine("General mail error:");
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
