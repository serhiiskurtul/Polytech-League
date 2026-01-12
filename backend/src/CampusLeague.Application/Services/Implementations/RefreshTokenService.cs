using CampusLeague.Domain.Entities;
using CampusLeague.Domain.Exceptions;
using CampusLeague.Application.Services.Interfaces;
using CampusLeague.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusLeague.Application.Services.Implementations;

public class RefreshTokenService 
    : AbstractEntityService<RefreshToken>, IRefreshTokenService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(ApplicationDbContext dbcontext, ILogger<RefreshTokenService> logger)
        : base(dbcontext, logger)
    {
        _dbcontext = dbcontext;
        _logger = logger;
    }

    public async Task<RefreshToken?> GetActiveTokenByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching active refresh token for userId {UserId}", userId);

        var tokens = await _dbcontext.Set<RefreshToken>()
            .AsNoTracking()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked) 
            .ToListAsync(cancellationToken); 

        var token = tokens
            .Where(rt => rt.IsActive)
            .OrderByDescending(rt => rt.ExpiresAt)
            .FirstOrDefault();

        if (token is null)
            _logger.LogWarning("No active refresh token found for userId {UserId}", userId);

        return token;
    }


    public async Task<RefreshToken> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching refresh token by value {Token}", token);

        var entity = await _dbcontext.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        if (entity is null)
        {
            _logger.LogWarning("Refresh token {Token} not found", token);
            throw new EntityNotFoundException(nameof(RefreshToken), token);
        }

        return entity;
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking refresh token {Token}", token);

        var entity = await _dbcontext.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        if (entity is null)
        {
            _logger.LogWarning("Refresh token {Token} not found for revoke", token);
            throw new EntityNotFoundException(nameof(RefreshToken), token);
        }

        entity.IsRevoked = true;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token {Token} successfully revoked", token);
    }

    protected override void UpdateValues(RefreshToken oldEntity, RefreshToken newEntity)
    {
        oldEntity.IsRevoked = newEntity.IsRevoked;
    }
}
