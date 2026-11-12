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

    private const int MaxVerificationAttempts = 5;

    public async Task<bool> VerifyAsync(string code, string email)
    {
        var key = "email_verification_code_" + email;
        var attemptsKey = "email_verification_attempts_" + email;

        var redisCode = await redisService.GetDataAsync<string>(key);

        if (redisCode == null) return false;
        if (code != redisCode)
        {
            // Після кількох невдалих спроб код анулюється, щоб його не можна було підібрати перебором
            var attempts = await redisService.GetDataAsync<int>(attemptsKey) + 1;
            if (attempts >= MaxVerificationAttempts)
            {
                await redisService.RemoveDataAsync(key);
                await redisService.RemoveDataAsync(attemptsKey);
            }
            else
            {
                await redisService.SetDataAsync(attemptsKey, attempts, TimeSpan.FromMinutes(10));
            }
            return false;
        }

        await redisService.RemoveDataAsync(key);
        await redisService.RemoveDataAsync(attemptsKey);
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
