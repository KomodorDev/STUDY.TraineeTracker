using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

/// <summary>
/// Implements the <see cref="IEmailSender"/> interface using Gmail's SMTP service to send HTML emails.
/// Uses settings injected via <see cref="EmailSettings"/> to authenticate and configure the sender.
/// </summary>
/// <remarks>
/// Code Ownership: Simon Hinterreiter (hintsimo)
/// </remarks>
public class GmailEmailSender : IEmailSender {
    private readonly EmailSettings _settings;

    // ------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="GmailEmailSender"/> class using injected email settings.
    /// </summary>
    /// <param name="settings">The email configuration bound from <c>appsettings.json</c>, injected via <see cref="IOptions{EmailSettings}"/>.</param>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public GmailEmailSender(IOptions<EmailSettings> settings) {
        _settings = settings.Value;
    }

    // ------------------------------------------------------
    /// <summary>
    /// Sends an HTML-formatted email using Gmail SMTP.
    /// </summary>
    /// <param name="email">The recipient's email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="htmlMessage">The HTML content of the email body.</param>
    /// <returns>A Task representing the asynchronous email sending operation.</returns>
    /// <exception cref="SmtpException">Thrown when the SMTP server returns an error.</exception>
    /// <exception cref="Exception">Thrown for unexpected errors during email sending.</exception>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
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
        };

        try {
            await smtp.SendMailAsync(message);
        }
        catch (SmtpException ex) {
            Console.WriteLine("SMTP error:");
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (Exception ex) {
            Console.WriteLine("General mail error:");
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    // ------------------------------------------------------
}
