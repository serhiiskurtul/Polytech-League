using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.BeachVolleyball;

public class BeachVolleyballMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BeachVolleyballMatch>> _logger
) :
    AbstractEntityService<BeachVolleyballMatch>(_dbcontext, _logger),
    IBeachVolleyballMatchService
{
    public override async Task<BeachVolleyballMatch> CreateAsync(BeachVolleyballMatch entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating beach volleyball match with rosters.");
        await MapRoster(entity, cancellationToken);
        _dbcontext.BeachVolleyballMatches.Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public override async Task UpdateAsync(long id, BeachVolleyballMatch newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating beach volleyball match with rosters. Id: {Id}", id);

        var oldEntity = await _dbcontext.BeachVolleyballMatches
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        
        if (oldEntity == null)
        {
            throw new EntityNotFoundException(nameof(BeachVolleyballMatch), id);
        }

        UpdateValues(oldEntity, newEntity);
        await MapRoster(oldEntity, newEntity, cancellationToken);

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    private async Task MapRoster(BeachVolleyballMatch target, BeachVolleyballMatch source, CancellationToken cancellationToken)
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

    private async Task MapRoster(BeachVolleyballMatch entity, CancellationToken cancellationToken)
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

    protected override void UpdateValues(BeachVolleyballMatch oldEntity, BeachVolleyballMatch newEntity)
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
