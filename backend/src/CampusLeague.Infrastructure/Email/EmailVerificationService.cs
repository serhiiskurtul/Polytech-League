using CampusLeague.Infrastructure.Redis;

namespace CampusLeague.Infrastructure.Email;

public class EmailVerificationService(IRedisService redisService) : IEmailVerificationService
{
    public string GenerateVerificationCode()
    {
        return new Random().Next(100000, 999999).ToString(); // 6 цифр
    }

    public async Task VerifyAsync(string code, string email)
    {
        var redisCode = await redisService.GetDataAsync<string>("email_verification_code_" + email);
        if (code != redisCode) throw new Exception("Verification Code is incorrect");

        await redisService.RemoveDataAsync("email_verification_code_" + email);
    }

}
