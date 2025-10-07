using CampusLeague.Application.Dto;

namespace CampusLeague.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> AuthenticateAsync(AuthRequest authRequest);

    Task RevokeRefreshTokenAsync(
        RefreshTokenRequest refreshToken, CancellationToken cancellationToken = default);

    Task<AuthResponse> RevokeAndRefreshJwtTokensAsync(
        RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default);
}
