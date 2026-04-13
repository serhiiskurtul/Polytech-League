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
    public override async Task<BadmintonMatch> CreateAsync(BadmintonMatch entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating badminton match with rosters.");
        await MapRoster(entity, cancellationToken);
        _dbcontext.BadmintonMatches.Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

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
        await MapRoster(oldEntity, newEntity, cancellationToken);

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    private async Task MapRoster(BadmintonMatch target, BadmintonMatch source, CancellationToken cancellationToken)
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

    private async Task MapRoster(BadmintonMatch entity, CancellationToken cancellationToken)
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
