namespace CampusLeague.Infrastructure.Email;

public interface IEmailSender
{
    Task SendEmailAsync(
        string email, string subject, string message, CancellationToken cancellationToken = default);
}
