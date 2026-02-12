using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IRefreshTokenService : IAbstractEntityService<RefreshToken>
{
    Task<RefreshToken?> GetActiveTokenByUserIdAsync(
        long userId, CancellationToken cancellationToken = default);

    Task<RefreshToken> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    Task RevokeAsync(string token, CancellationToken cancellationToken = default);
}
