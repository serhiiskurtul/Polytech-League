namespace CampusLeague.Infrastructure.Email;

public interface IEmailVerificationService
{
    string GenerateVerificationCode();

    Task<bool> VerifyAsync(string code, string email);
    
    Task<bool> VerifyAndConfirmEmailAsync(string code, string email, CancellationToken cancellationToken = default);
}
