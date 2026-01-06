namespace CampusLeague.Infrastructure.Email;

public interface IEmailVerificationService
{
    string GenerateVerificationCode();

    Task<bool> VerifyAsync(string code, string email);
}
