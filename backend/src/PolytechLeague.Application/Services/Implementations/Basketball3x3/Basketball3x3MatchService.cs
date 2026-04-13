using PolytechLeague.Domain.Entities.Basketball3x3;
using PolytechLeague.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.Basketball3x3;

public class Basketball3x3MatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<Basketball3x3Match>> _logger
) :
    AbstractEntityService<Basketball3x3Match>(_dbcontext, _logger),
    IBasketball3x3MatchService
{
    public override async Task<Basketball3x3Match> CreateAsync(Basketball3x3Match entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating basketball 3x3 match with rosters.");
        await MapRoster(entity, cancellationToken);
        _dbcontext.Basketball3x3Matches.Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public override async Task UpdateAsync(long id, Basketball3x3Match newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating basketball 3x3 match with rosters. Id: {Id}", id);

        var oldEntity = await _dbcontext.Basketball3x3Matches
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        
        if (oldEntity == null)
        {
            throw new EntityNotFoundException(nameof(Basketball3x3Match), id);
        }

        UpdateValues(oldEntity, newEntity);
        await MapRoster(oldEntity, newEntity, cancellationToken);

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    private async Task MapRoster(Basketball3x3Match target, Basketball3x3Match source, CancellationToken cancellationToken)
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

    private async Task MapRoster(Basketball3x3Match entity, CancellationToken cancellationToken)
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

    protected override void UpdateValues(Basketball3x3Match oldEntity, Basketball3x3Match newEntity)
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
