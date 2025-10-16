using CampusLeague.Application.Dto;
using CampusLeague.Infrastructure.Email;
using CampusLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("email")]
public class EmailController(
    IEmailVerificationService emailVerificationService,
    EmailSender emailSender,
    IRedisService redisService) : Controller
{
    [AllowAnonymous]
    [HttpPost("verify")]
    public async Task<IActionResult> Verify(
        [FromBody] EmailVerificationRequest emailVerificationRequest)
    {
        await emailVerificationService.VerifyAsync(
            emailVerificationRequest.Code, emailVerificationRequest.Email);

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("send-verification-email")]
    public async Task<IActionResult> SendVerificationEmail(
        [FromBody] string email, CancellationToken cancellationToken)
    {
        var code = emailVerificationService.GenerateVerificationCode();

        await emailSender.SendEmailAsync(
            email, "Підтвердження пошти", $"Ваш код для підтвердження: {code}", cancellationToken);

        await redisService.SetDataAsync<string>(
            "email_verification_code_" + email, code, TimeSpan.FromMinutes(10));

        return Ok();
    }
}
