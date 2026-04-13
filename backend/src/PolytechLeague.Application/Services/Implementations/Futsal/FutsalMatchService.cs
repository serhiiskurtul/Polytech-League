using PolytechLeague.Domain.Entities.Futsal;
using PolytechLeague.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.Futsal;

public class FutsalMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<FutsalMatch>> _logger
) :
    AbstractEntityService<FutsalMatch>(_dbcontext, _logger),
    IFutsalMatchService
{
    public override async Task<FutsalMatch> CreateAsync(FutsalMatch entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating futsal match with rosters.");
        await MapRoster(entity, cancellationToken);
        _dbcontext.FutsalMatches.Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public override async Task UpdateAsync(long id, FutsalMatch newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating futsal match with rosters. Id: {Id}", id);

        var oldEntity = await _dbcontext.FutsalMatches
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        
        if (oldEntity == null)
        {
            throw new EntityNotFoundException(nameof(FutsalMatch), id);
        }

        UpdateValues(oldEntity, newEntity);
        await MapRoster(oldEntity, newEntity, cancellationToken);

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    private async Task MapRoster(FutsalMatch target, FutsalMatch source, CancellationToken cancellationToken)
    {
        if (source.HomeRosterIds != null)
        {
            target.HomeRoster = await _dbcontext.Players
                .Where(p => source.HomeRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        if (source.AwayRosterIds != null)
        {
            target.AwayRoster = await _dbcontext.Players
                .Where(p => source.AwayRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }
    }

    private async Task MapRoster(FutsalMatch entity, CancellationToken cancellationToken)
    {
        if (entity.HomeRosterIds != null)
        {
            entity.HomeRoster = await _dbcontext.Players
                .Where(p => entity.HomeRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        if (entity.AwayRosterIds != null)
        {
            entity.AwayRoster = await _dbcontext.Players
                .Where(p => entity.AwayRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }
    }

    protected override void UpdateValues(FutsalMatch oldEntity, FutsalMatch newEntity)
    {
        oldEntity.EventId = newEntity.EventId;
        oldEntity.HomeTeamId = newEntity.HomeTeamId;
        oldEntity.AwayTeamId = newEntity.AwayTeamId;
        oldEntity.HomeTeamScore = newEntity.HomeTeamScore;
        oldEntity.AwayTeamScore = newEntity.AwayTeamScore;
        oldEntity.WinnerTeamId = newEntity.WinnerTeamId;
        oldEntity.Stage = newEntity.Stage;
    }
}
