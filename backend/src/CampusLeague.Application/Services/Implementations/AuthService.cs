using System.Security.Authentication;
using CampusLeague.Application.Dto;
using CampusLeague.Application.JWT;
using CampusLeague.Application.Security;
using CampusLeague.Application.Services.Interfaces;
using CampusLeague.Domain.Entities;
using CampusLeague.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace CampusLeague.Application.Services.Implementations;

public class AuthService(
    ApplicationDbContext dbcontext,
    IPasswordEncoder passwordService,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService,
    IUserService userService) : IAuthService
{
    public async Task<AuthResponse> AuthenticateAsync(AuthRequest authRequest)
    {
        var user = await dbcontext.Set<User>().AsNoTracking().FirstOrDefaultAsync(x => x.Email == authRequest.Email);
        if (user == null || !passwordService.Verify(authRequest.Password, user.Password))
        {
            throw new AuthenticationException("Invalid email or password");
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

        oldRefreshToken.IsRevoked = true;
        await refreshTokenService.UpdateAsync(
            oldRefreshToken.Id, oldRefreshToken, cancellationToken);

        var user = await userService.GetByIdAsync(oldRefreshToken.UserId, cancellationToken);

        var newAccessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken = jwtService.GenerateRefreshToken(user.Id);

        await refreshTokenService.CreateAsync(newRefreshToken, cancellationToken);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }
}