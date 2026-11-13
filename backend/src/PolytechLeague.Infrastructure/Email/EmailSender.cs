using System.Net;
using System.Net.Mail;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace PolytechLeague.Infrastructure.Email;

public class EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger) : IEmailSender
{
    private static readonly HttpClient _httpClient = new();

    public async Task SendEmailAsync(
        string email, string subject, string message, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(emailSettings.Value.SmtpHost) || string.IsNullOrEmpty(emailSettings.Value.ApiKey))
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
                    new { type = "text/plain", value = ToPlainText(message) },
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
                logger.LogError("SendGrid API returned error: {StatusCode}. Details: {Error}",
                    response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email} via SendGrid", email);
        }
    }

    private async Task SendViaSmtpFallback(string email, string subject, string message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(emailSettings.Value.Email) || string.IsNullOrEmpty(emailSettings.Value.Password))
        {
            logger.LogWarning("Email settings missing (SMTP fallback). Email to {Email} was not sent", email);
            return;
        }

        var settings = emailSettings.Value;
        var host = string.IsNullOrEmpty(settings.SmtpHost) ? "smtp.gmail.com" : settings.SmtpHost;
        var user = string.IsNullOrEmpty(settings.SmtpUser) ? settings.Email : settings.SmtpUser;

        using var client = new SmtpClient(host, settings.SmtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(user, settings.Password)
        };

        using var mailMessage = new MailMessage(
            new MailAddress(settings.Email, settings.FromName ?? "Polytech League"),
            new MailAddress(email))
        {
            Subject = subject
        };
        // Дві версії листа: чистий текст і HTML, пошта сама показує HTML
        mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            ToPlainText(message), System.Text.Encoding.UTF8, "text/plain"));
        mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            message, System.Text.Encoding.UTF8, "text/html"));

        try
        {
            logger.LogInformation("Attempting SMTP via {Host} to {Email} (Timeout 10s)...", host, email);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            await client.SendMailAsync(mailMessage, cts.Token);
            logger.LogInformation("Email successfully sent to {Email} via SMTP", email);
        }
        catch (Exception ex)
        {
            logger.LogError("SMTP Failed for {Email}: {Error}", email, ex.Message);
        }
    }

    // Текстова версія листа без HTML-тегів: сирий HTML у text/plain спам-фільтри вважають ознакою спаму
    private static string ToPlainText(string message)
    {
        var text = Regex.Replace(message, @"<(br|/p|/div|/h[1-6])\s*/?>", "\n", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, "<[^>]+>", string.Empty);
        text = WebUtility.HtmlDecode(text);
        text = Regex.Replace(text, @"[ \t]+", " ");
        text = Regex.Replace(text, @"\s*\n\s*", "\n");
        return text.Trim();
    }
}
