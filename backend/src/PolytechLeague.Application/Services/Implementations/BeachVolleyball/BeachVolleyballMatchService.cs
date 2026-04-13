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
