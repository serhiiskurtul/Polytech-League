using PolytechLeague.Application.Dto;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> AuthenticateAsync(AuthRequest authRequest);

    Task RevokeRefreshTokenAsync(
        RefreshTokenRequest refreshToken, CancellationToken cancellationToken = default);

    Task<AuthResponse> RevokeAndRefreshJwtTokensAsync(
        RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default);

    Task SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken = default);
    
    Task ResetPasswordAsync(string email, string code, string newPassword, CancellationToken cancellationToken = default);
}
