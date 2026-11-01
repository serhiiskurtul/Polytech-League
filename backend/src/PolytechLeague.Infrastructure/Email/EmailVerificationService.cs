using System.Security.Cryptography;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.Infrastructure.Email;

public class EmailVerificationService(
    IRedisService redisService, IUserService userService) : IEmailVerificationService
{
    public string GenerateVerificationCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString(); // 6 цифр
    }

    public async Task<bool> VerifyAsync(string code, string email)
    {
        var redisCode = await redisService.GetDataAsync<string>(
            "email_verification_code_" + email
        );

        if (redisCode == null) return false;
        if (code != redisCode) return false;

        await redisService.RemoveDataAsync("email_verification_code_" + email);
        return true;
    }

    public async Task<bool> VerifyAndConfirmEmailAsync(string code, string email, CancellationToken cancellationToken = default)
    {
        var isValid = await VerifyAsync(code, email);

        if (!isValid) return false;

        // Підтверджуємо email у базі
        await userService.ConfirmEmailAsync(email, cancellationToken);

        // Скидаємо кеш акаунта, інакше /users/me віддаватиме старий isEmailConfirmed
        var user = await userService.GetByEmailAsync(email, cancellationToken);
        await redisService.RemoveDataAsync($"user_account_{user.Id}");
        return true;
    }
}
