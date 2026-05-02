using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace PolytechLeague.API.Controllers.Account;

[ApiController]
[Route("api")]
public class AuthController(IAuthService authService) : Controller
{
    [EnableRateLimiting("fixed")]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
    {
        var tokens = await authService.AuthenticateAsync(authRequest);

        SetAuthCookies(tokens);

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

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

            return Ok();
        }
        catch
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Unauthorized();
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

    private void SetAuthCookies(AuthResponse tokens)
    {
        var isProd = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT"));

        Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProd,
            SameSite = isProd ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(24),
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
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/"
            });
    }
}
