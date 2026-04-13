using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.Volleyball;

public class VolleyballMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<VolleyballMatch>> _logger
) :
    AbstractEntityService<VolleyballMatch>(_dbcontext, _logger),
    IVolleyballMatchService
{
    public override async Task<VolleyballMatch> CreateAsync(VolleyballMatch entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating volleyball match with rosters.");
        await MapRoster(entity, cancellationToken);
        _dbcontext.VolleyballMatches.Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public override async Task UpdateAsync(long id, VolleyballMatch newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating volleyball match with rosters. Id: {Id}", id);

        var oldEntity = await _dbcontext.VolleyballMatches
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        
        if (oldEntity == null)
        {
            throw new EntityNotFoundException(nameof(VolleyballMatch), id);
        }

        UpdateValues(oldEntity, newEntity);
        await MapRoster(oldEntity, newEntity, cancellationToken);

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    private async Task MapRoster(VolleyballMatch target, VolleyballMatch source, CancellationToken cancellationToken)
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

    private async Task MapRoster(VolleyballMatch entity, CancellationToken cancellationToken)
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

    protected override void UpdateValues(VolleyballMatch oldEntity, VolleyballMatch newEntity)
    {
        oldEntity.EventId = newEntity.EventId;
        oldEntity.HomeTeamId = newEntity.HomeTeamId;
        oldEntity.AwayTeamId = newEntity.AwayTeamId;
        oldEntity.SetsToWin = newEntity.SetsToWin;
        oldEntity.HomeSetsWon = newEntity.HomeSetsWon;
        oldEntity.AwaySetsWon = newEntity.AwaySetsWon;
        oldEntity.WinnerTeamId = newEntity.WinnerTeamId;
        oldEntity.Stage = newEntity.Stage;
    }
}
