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
        // 1. Bulk delete associated participants using a subquery
        await _dbcontext.FutsalEventParticipants
            .Where(p => p.EventId == _dbcontext.FutsalTeams
                .Where(t => t.Id == id)
                .Select(t => t.EventId)
                .FirstOrDefault() &&
                _dbcontext.Players.Any(pl => pl.Id == p.PlayerId && pl.FutsalTeamId == id))
            .ExecuteDeleteAsync(cancellationToken);

        // 2. Clear FutsalTeamId for all players in this team
        await _dbcontext.Players
            .Where(p => p.FutsalTeamId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.FutsalTeamId, (long?)null), cancellationToken);

        // 3. Delete the team itself using base logic
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
