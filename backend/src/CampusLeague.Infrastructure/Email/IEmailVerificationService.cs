namespace CampusLeague.Infrastructure.Email;

public interface IEmailVerificationService
{
    string GenerateVerificationCode();

    Task VerifyAsync(string code, string email);
}
