using CampusLeague.Application.Dto;
using CampusLeague.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
public class AuthController(IAuthService authService) : Controller
{
    [EnableRateLimiting("fixed")]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest)
    {
        var tokens = await authService.AuthenticateAsync(authRequest);

        Response.Cookies.Append(
            "access_token",
            tokens.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,              // false тільки локально без https
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

        Response.Cookies.Append(
            "refresh_token",
            tokens.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

        return Ok();
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

}
