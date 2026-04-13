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

        if (newEntity.HomeRosterIds != null)
        {
            var homePlayers = await _dbcontext.Players
                .Where(p => newEntity.HomeRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            oldEntity.HomeRoster = homePlayers;
        }

        if (newEntity.AwayRosterIds != null)
        {
            var awayPlayers = await _dbcontext.Players
                .Where(p => newEntity.AwayRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            oldEntity.AwayRoster = awayPlayers;
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
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
