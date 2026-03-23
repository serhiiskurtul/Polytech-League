using PolytechLeague.Application.Dto;
using PolytechLeague.Infrastructure.Email;
using PolytechLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.Common;

[ApiController]
[Route("email")]
public class EmailController(
    IEmailVerificationService emailVerificationService,
    IEmailSender emailSender,
    IRedisService redisService) : Controller
{
    [AllowAnonymous]
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] EmailVerificationRequest request)
    {
        var success = await emailVerificationService.VerifyAndConfirmEmailAsync(
            request.Code, request.Email);

        if (!success)
            return BadRequest("Invalid verification code");

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("send-verification-email")]
    public async Task<IActionResult> SendVerificationEmail(
        [FromBody] SendEmailRequest request, CancellationToken cancellationToken)
    {
        var code = emailVerificationService.GenerateVerificationCode();

        await emailSender.SendEmailAsync(
            request.Email,
            "Підтвердження пошти",
            $"Ваш код для підтвердження: {code}",
            cancellationToken);

        await redisService.SetDataAsync<string>(
            "email_verification_code_" + request.Email, code, TimeSpan.FromMinutes(10));

        return Ok();
    }
}
