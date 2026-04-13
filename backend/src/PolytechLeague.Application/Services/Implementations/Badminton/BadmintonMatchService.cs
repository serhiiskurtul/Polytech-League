using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonMatch>> _logger
) :
    AbstractEntityService<BadmintonMatch>(_dbcontext, _logger),
    IBadmintonMatchService
{
    public override async Task UpdateAsync(long id, BadmintonMatch newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating badminton match with rosters. Id: {Id}", id);

        var oldEntity = await _dbcontext.BadmintonMatches
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        
        if (oldEntity == null)
        {
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(BadmintonMatch), id);
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

    protected override void UpdateValues(BadmintonMatch oldEntity, BadmintonMatch newEntity)
    {
        oldEntity.WinnerPlayerId = newEntity.WinnerPlayerId;
        oldEntity.WinnerTeamId = newEntity.WinnerTeamId;
        oldEntity.Team1Id = newEntity.Team1Id;
        oldEntity.Team2Id = newEntity.Team2Id;
        oldEntity.Player1Id = newEntity.Player1Id;
        oldEntity.Player2Id = newEntity.Player2Id;
        oldEntity.HomeScore = newEntity.HomeScore;
        oldEntity.AwayScore = newEntity.AwayScore;
        oldEntity.Stage = newEntity.Stage;
        oldEntity.CourtNumber = newEntity.CourtNumber;
        oldEntity.SetsToWin = newEntity.SetsToWin;
    }
}
