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
        var team = await _dbcontext.Set<FutsalTeam>()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team != null)
        {
            if (team.Players != null)
            {
                var playerIds = team.Players.Select(p => p.Id).ToList();
                
                // Also remove these players from the event participants
                var participants = await _dbcontext.Set<FutsalEventParticipant>()
                    .Where(p => p.EventId == team.EventId && playerIds.Contains(p.PlayerId))
                    .ToListAsync(cancellationToken);
                
                if (participants.Any())
                {
                    _dbcontext.Set<FutsalEventParticipant>().RemoveRange(participants);
                }

                foreach (var player in team.Players)
                {
                    player.FutsalTeamId = null;
                }
            }
            await _dbcontext.SaveChangesAsync(cancellationToken);
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
