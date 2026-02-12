using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace PolytechLeague.Infrastructure.Email;

public class EmailSender(IOptions<EmailSettings> emailSettings) : IEmailSender
{
    public Task SendEmailAsync(
        string email, string subject, string message, CancellationToken cancellationToken = default)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(emailSettings.Value.Email, emailSettings.Value.Password)
        };

        var mailMessage = new MailMessage(
            from: emailSettings.Value.Email,
            to: email,
            subject,
            message
        );

        return client.SendMailAsync(mailMessage, cancellationToken);
    }
}
