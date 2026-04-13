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
