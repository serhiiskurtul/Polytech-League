using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace PolytechLeague.Infrastructure.Email;

public class EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(
        string email, string subject, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(emailSettings.Value.Email) || string.IsNullOrEmpty(emailSettings.Value.Password))
        {
            logger.LogWarning("Email settings are missing. [DEBUG] Verification email to {Email}: {Message}", email, message);
            return;
        }

        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(emailSettings.Value.Email, emailSettings.Value.Password)
        };

        var mailSender = emailSettings.Value.Email;
        var mailMessage = new MailMessage(
            from: mailSender,
            to: email,
            subject,
            message
        );

        try
        {
            await client.SendMailAsync(mailMessage, cancellationToken);
            logger.LogInformation("Email successfully sent to {Email}", email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}. [DEBUG] Verification code: {Message}", email, message);
            // We don't rethrow to avoid 500 errors in the API, especially in dev
        }
    }
}
