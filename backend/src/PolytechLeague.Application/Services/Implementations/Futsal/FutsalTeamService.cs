using PolytechLeague.Domain.Entities.Futsal;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.Futsal;

public class FutsalTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<FutsalTeam>> _logger
) :
    AbstractEntityService<FutsalTeam>(_dbcontext, _logger),
    IFutsalTeamService
{
    public override async Task UpdateAsync(long id, FutsalTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.Set<FutsalTeam>()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        if (oldEntity == null) return;

        UpdateValues(oldEntity, newEntity);

        if (newEntity.PlayerIds != null)
        {
            var players = await _dbcontext.Set<Player>()
                .Where(p => newEntity.PlayerIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            
            oldEntity.Players = players;
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public override async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var teamInfo = await _dbcontext.FutsalTeams
            .AsNoTracking()
            .Select(t => new { t.Id, t.EventId, PlayerIds = t.Players!.Select(p => p.Id).ToList() })
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (teamInfo != null && teamInfo.PlayerIds.Any())
        {
            // 1. Bulk delete associated participants
            await _dbcontext.FutsalEventParticipants
                .Where(p => p.EventId == teamInfo.EventId && teamInfo.PlayerIds.Contains(p.PlayerId))
                .ExecuteDeleteAsync(cancellationToken);

            // 2. Clear FutsalTeamId for all players in this team
            await _dbcontext.Players
                .Where(p => p.FutsalTeamId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.FutsalTeamId, (long?)null), cancellationToken);
        }

        await base.DeleteAsync(id, cancellationToken);
    }

    protected override void UpdateValues(FutsalTeam oldEntity, FutsalTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Points = newEntity.Points;
        oldEntity.CaptainId = newEntity.CaptainId;
        oldEntity.GoalkeeperId = newEntity.GoalkeeperId;
    }
}
