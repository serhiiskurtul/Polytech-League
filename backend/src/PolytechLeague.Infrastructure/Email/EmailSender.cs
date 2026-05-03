using System.Net;
using System.Net.Mail;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace PolytechLeague.Infrastructure.Email;

public class EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger) : IEmailSender
{
    private static readonly HttpClient _httpClient = new();

    public async Task SendEmailAsync(
        string email, string subject, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(emailSettings.Value.ApiKey))
        {
            await SendViaSmtpFallback(email, subject, message, cancellationToken);
            return;
        }

        await SendViaSendGrid(email, subject, message, cancellationToken);
    }

    private async Task SendViaSendGrid(string email, string subject, string message, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Attempting to send email to {Email} via SendGrid API...", email);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", emailSettings.Value.ApiKey);

            var body = new
            {
                personalizations = new[]
                {
                    new { to = new[] { new { email = email } } }
                },
                from = new { 
                    email = emailSettings.Value.Email,
                    name = emailSettings.Value.FromName ?? "Polytech League"
                },
                subject = subject,
                content = new[]
                {
                    new { type = "text/plain", value = message },
                    new { type = "text/html", value = message.Contains("<html>") || message.Contains("</div>") || message.Contains("<p>") 
                        ? message 
                        : $@"
                        <div style='font-family: sans-serif; padding: 20px; color: #333;'>
                            <h2 style='color: #2563eb;'>Polytech League</h2>
                            <p style='font-size: 16px;'>{message}</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />
                            <p style='font-size: 12px; color: #666;'>Це автоматичне повідомлення. Будь ласка, не відповідайте на нього.</p>
                        </div>" 
                    }
                }
            };

            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Email successfully sent to {Email} via SendGrid", email);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("SendGrid API returned error: {StatusCode}. Details: {Error}. [DEBUG] Verification code: {Code}", 
                    response.StatusCode, errorContent, message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email} via SendGrid. [DEBUG] Verification code: {Code}", email, message);
        }
    }

    private async Task SendViaSmtpFallback(string email, string subject, string message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(emailSettings.Value.Email) || string.IsNullOrEmpty(emailSettings.Value.Password))
        {
            logger.LogWarning("Email settings missing (SMTP fallback). [DEBUG] Verification email to {Email}: {Message}", email, message);
            return;
        }

        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(emailSettings.Value.Email, emailSettings.Value.Password)
        };

        var mailMessage = new MailMessage(from: emailSettings.Value.Email, to: email, subject, message)
        {
            IsBodyHtml = true
        };

        try
        {
            logger.LogInformation("Attempting SMTP fallback to {Email} (Timeout 10s)...", email);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            await client.SendMailAsync(mailMessage, cts.Token);
            logger.LogInformation("Email successfully sent to {Email} via SMTP", email);
        }
        catch (Exception ex)
        {
            logger.LogError("SMTP Failed: {Error}. [DEBUG] Verification code: {Code}", ex.Message, message);
        }
    }
}
