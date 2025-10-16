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
        AuthResponse tokens = await authService.AuthenticateAsync(authRequest);

        HttpContext.Response.Cookies.Append("some-cookies", tokens.AccessToken);
        HttpContext.Response.Cookies.Append("deliciously-cookies", tokens.RefreshToken);

        return Ok(tokens);
    }

    [Authorize]
    [HttpPost("logout")] 
    public async Task<IActionResult> LogOutAsync([FromBody] RefreshTokenRequest refreshToken)
    {
        await authService.RevokeRefreshTokenAsync(refreshToken);
        return NoContent();
    }
}
