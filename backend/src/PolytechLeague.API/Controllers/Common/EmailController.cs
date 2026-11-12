using PolytechLeague.Application.Dto;
using PolytechLeague.Infrastructure.Email;
using PolytechLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.Common;

[ApiController]
[Route("api/email")]
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
            "Підтвердження пошти - Polytech League",
            $@"
            <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e5e7eb; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #2563eb; padding: 20px; text-align: center;'>
                    <h1 style='color: white; margin: 0; font-size: 24px;'>Polytech League</h1>
                </div>
                <div style='padding: 30px; line-height: 1.6; color: #374151;'>
                    <p style='font-size: 18px; margin-bottom: 20px;'>Вітаємо!</p>
                    <p>Ви отримали це повідомлення, тому що цей email був вказаний при реєстрації або зміні даних у <b>Polytech League</b>.</p>
                    <p>Для підтвердження пошти, будь ласка, використовуйте наступний код:</p>
                    <div style='background-color: #f3f4f6; padding: 20px; text-align: center; border-radius: 6px; margin: 30px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #1f2937;'>{code}</span>
                    </div>
                    <p style='font-size: 14px; color: #6b7280;'>Код дійсний протягом 10 хвилин. Якщо ви не робили цього запиту, просто проігноруйте цей лист.</p>
                </div>
                <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb; font-size: 12px; color: #9ca3af;'>
                    &copy; {DateTime.Now.Year} Polytech League. Всі права захищені.
                </div>
            </div>",
            cancellationToken);

        await redisService.SetDataAsync<string>(
            "email_verification_code_" + request.Email, code, TimeSpan.FromMinutes(10));
        await redisService.RemoveDataAsync("email_verification_attempts_" + request.Email);

        return Ok();
    }
}
