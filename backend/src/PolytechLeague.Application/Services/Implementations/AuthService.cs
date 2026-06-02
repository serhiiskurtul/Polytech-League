using System.Security.Authentication;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.JWT;
using PolytechLeague.Application.Security;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations;

public class AuthService(
    ApplicationDbContext dbcontext,
    IPasswordEncoder passwordService,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService,
    IUserService userService) : IAuthService
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

        oldRefreshToken.IsRevoked = true;
        await refreshTokenService.UpdateAsync(
            oldRefreshToken.Id, oldRefreshToken, cancellationToken);

        var user = await userService.GetByIdAsync(oldRefreshToken.UserId, cancellationToken);

        if (user.IsBlocked)
        {
            throw new AuthenticationException("Your account is blocked. Please contact support.");
        }

        var newAccessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken = jwtService.GenerateRefreshToken(user.Id);

        await refreshTokenService.CreateAsync(newRefreshToken, cancellationToken);

        Console.WriteLine($"Token: {oldRefreshToken.Token}");
        Console.WriteLine($"IsExpired: {oldRefreshToken.IsExpired}");
        Console.WriteLine($"IsRevoked: {oldRefreshToken.IsRevoked}");
        Console.WriteLine($"ExpiresAt: {oldRefreshToken.ExpiresAt}");
        Console.WriteLine($"Now: {DateTime.UtcNow}");

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }
}