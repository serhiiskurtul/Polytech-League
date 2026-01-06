using System.Security.Cryptography;
using CampusLeague.Infrastructure.Redis;

namespace CampusLeague.Infrastructure.Email;

public class EmailVerificationService(IRedisService redisService) : IEmailVerificationService
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


}
