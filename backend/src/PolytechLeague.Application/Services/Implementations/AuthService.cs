using System.Security.Authentication;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.JWT;
using PolytechLeague.Application.Security;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

using PolytechLeague.Infrastructure.Redis;
using PolytechLeague.Infrastructure.Email;

namespace PolytechLeague.Application.Services.Implementations;

public class AuthService(
    ApplicationDbContext dbcontext,
    IPasswordEncoder passwordService,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService,
    IUserService userService,
    IRedisService redisService,
    IEmailSender emailSender) : IAuthService
{
    public async Task<AuthResponse> AuthenticateAsync(AuthRequest authRequest)
    {
        var user = await dbcontext.Set<User>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email != null && x.Email.ToLower() == authRequest.Email.ToLower());
        if (user == null || !passwordService.Verify(authRequest.Password, user.Password))
        {
            throw new AuthenticationException("Invalid email or password");
        }

        if (user.IsBlocked)
        {
            throw new AuthenticationException("Your account is blocked. Please contact support.");
        }

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken(user.Id);

        await refreshTokenService.CreateAsync(refreshToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task RevokeRefreshTokenAsync(
        RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        var tokenFound = await refreshTokenService.GetByRefreshTokenAsync(
            refreshTokenRequest.RefreshToken, cancellationToken);
        if (tokenFound.IsRevoked) throw new AuthenticationException("Token is already revoked");

        tokenFound.IsRevoked = true;
        await refreshTokenService.UpdateAsync(tokenFound.Id, tokenFound, cancellationToken);
    }

    public async Task<AuthResponse> RevokeAndRefreshJwtTokensAsync (
        RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        var oldRefreshToken = await refreshTokenService.GetByRefreshTokenAsync(
            refreshTokenRequest.RefreshToken, cancellationToken);

        if (oldRefreshToken.IsExpired)
        {
            throw new AuthenticationException("Refresh token is expired.");
        }

        if (oldRefreshToken.IsRevoked)
        {
            throw new AuthenticationException("Refresh token is revoked.");
        }

        var user = await userService.GetByIdAsync(oldRefreshToken.UserId, cancellationToken);

        if (user.IsBlocked)
        {
            throw new AuthenticationException("Your account is blocked. Please contact support.");
        }

        var newAccessToken = jwtService.GenerateAccessToken(user);

        Console.WriteLine($"Token: {oldRefreshToken.Token}");
        Console.WriteLine($"IsExpired: {oldRefreshToken.IsExpired}");
        Console.WriteLine($"IsRevoked: {oldRefreshToken.IsRevoked}");
        Console.WriteLine($"ExpiresAt: {oldRefreshToken.ExpiresAt}");
        Console.WriteLine($"Now: {DateTime.UtcNow}");

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = oldRefreshToken.Token
        };
    }

    public async Task SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetByEmailAsync(email, cancellationToken);
        
        // Generate a 6-digit code
        var code = new Random().Next(100000, 999999).ToString();
        var key = "password_reset_code_" + email.ToLower();
        
        await redisService.SetDataAsync(key, code, TimeSpan.FromMinutes(15));
        
        await emailSender.SendEmailAsync(
            email, 
            "Polytech League - Відновлення пароля", 
            $"Ваш код для відновлення пароля: {code}. Код дійсний 15 хвилин.", 
            cancellationToken);
    }

    public async Task ResetPasswordAsync(string email, string code, string newPassword, CancellationToken cancellationToken = default)
    {
        var key = "password_reset_code_" + email.ToLower();
        var redisCode = await redisService.GetDataAsync<string>(key);
        
        if (string.IsNullOrEmpty(redisCode) || redisCode != code)
        {
            throw new AuthenticationException("Невірний або протермінований код відновлення");
        }
        
        var user = await userService.GetByEmailAsync(email, cancellationToken);
        await userService.UpdatePasswordAsync(user.Id, newPassword, cancellationToken);
        
        await redisService.RemoveDataAsync(key);
    }
}