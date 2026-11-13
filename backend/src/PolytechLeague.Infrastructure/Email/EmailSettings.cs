namespace PolytechLeague.Infrastructure.Email;

public class EmailSettings
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? ApiKey { get; set; }
    public string? FromName { get; set; }

    // SMTP-сервер (наприклад smtp.resend.com). Якщо заданий, листи йдуть через нього, а не через SendGrid
    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    // Логін SMTP; якщо порожній, використовується Email (як для Gmail)
    public string? SmtpUser { get; set; }
}
