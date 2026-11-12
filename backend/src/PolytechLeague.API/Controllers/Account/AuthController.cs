using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Authentication;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Infrastructure.Redis;
using PolytechLeague.Infrastructure.Email;

namespace PolytechLeague.API.Controllers.Account;

[ApiController]
[Route("api")]
public class AuthController(
    IAuthService authService,
    IUserService userService,
    IRedisService redisService,
    IEmailSender emailSender) : Controller
{
    [EnableRateLimiting("fixed")]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
    {
        var tokens = await authService.AuthenticateAsync(authRequest);

        SetAuthCookies(tokens);

        return Ok(tokens);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest? bodyRequest, CancellationToken cancellationToken)
    {
        // Спробуємо взяти з тіла, якщо куки заблоковані
        var refreshToken = bodyRequest?.RefreshToken ?? Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        try
        {
            var tokens = await authService.RevokeAndRefreshJwtTokensAsync(
                new RefreshTokenRequest { RefreshToken = refreshToken },
                cancellationToken);

            SetAuthCookies(tokens);

            return Ok(tokens);
        }
        catch (Exception ex) when (ex is AuthenticationException || ex is EntityNotFoundException)
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Unauthorized();
        }
        catch (Exception ex)
        {
            // Do not delete cookies on transient errors
            return StatusCode(500, "An error occurred during token refresh.");
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await authService.RevokeRefreshTokenAsync(
                new RefreshTokenRequest { RefreshToken = refreshToken },
                cancellationToken);
        }

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("auth/forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userService.GetByEmailAsync(request.Email, cancellationToken);
        
            var code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var key = "password_reset_code_" + request.Email.ToLower();

            await redisService.SetDataAsync(key, code, TimeSpan.FromMinutes(15));
            await redisService.RemoveDataAsync("password_reset_attempts_" + request.Email.ToLower());
            
            await emailSender.SendEmailAsync(
                request.Email, 
                "Polytech League - Відновлення пароля", 
                $"Ваш код для відновлення пароля: {code}. Код дійсний 15 хвилин.", 
                cancellationToken);
                
            return Ok();
        }
        catch (EntityNotFoundException)
        {
            return Ok();
        }
    }

    [AllowAnonymous]
    [HttpPost("auth/verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest request)
    {
        if (!await IsResetCodeValidAsync(request.Email, request.Code))
        {
            return BadRequest(new { detail = "Невірний або протермінований код" });
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("auth/reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var key = "password_reset_code_" + request.Email.ToLower();

            if (!await IsResetCodeValidAsync(request.Email, request.Code))
            {
                throw new AuthenticationException("Невірний або протермінований код відновлення");
            }

            var user = await userService.GetByEmailAsync(request.Email, cancellationToken);
            await userService.UpdatePasswordAsync(user.Id, request.NewPassword, cancellationToken);

            await redisService.RemoveDataAsync(key);
            await redisService.RemoveDataAsync("password_reset_attempts_" + request.Email.ToLower());

            return Ok();
        }
        catch (AuthenticationException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }

    private const int MaxResetCodeAttempts = 5;

    // Після кількох невдалих спроб код анулюється, щоб його не можна було підібрати перебором
    private async Task<bool> IsResetCodeValidAsync(string email, string code)
    {
        var key = "password_reset_code_" + email.ToLower();
        var attemptsKey = "password_reset_attempts_" + email.ToLower();

        var redisCode = await redisService.GetDataAsync<string>(key);
        if (string.IsNullOrEmpty(redisCode)) return false;
        if (redisCode == code) return true;

        var attempts = await redisService.GetDataAsync<int>(attemptsKey) + 1;
        if (attempts >= MaxResetCodeAttempts)
        {
            await redisService.RemoveDataAsync(key);
            await redisService.RemoveDataAsync(attemptsKey);
        }
        else
        {
            await redisService.SetDataAsync(attemptsKey, attempts, TimeSpan.FromMinutes(15));
        }

        return false;
    }

    private void SetAuthCookies(AuthResponse tokens)
    {
        var isProd = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

        Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProd,
            SameSite = isProd ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60),
            Path = "/"
        });

        Response.Cookies.Append(
            "refresh_token",
            tokens.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = isProd,
                SameSite = isProd ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                Path = "/"
            });
    }
}
