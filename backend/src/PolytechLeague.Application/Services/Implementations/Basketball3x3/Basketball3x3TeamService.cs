using PolytechLeague.Domain.Entities.Basketball3x3;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.Basketball3x3;

public class Basketball3x3TeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<Basketball3x3Team>> _logger
) :
    AbstractEntityService<Basketball3x3Team>(_dbcontext, _logger),
    IBasketball3x3TeamService
{
    public override async Task UpdateAsync(long id, Basketball3x3Team newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.Set<Basketball3x3Team>()
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
        var team = await _dbcontext.Set<Basketball3x3Team>()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team != null)
        {
            if (team.Players != null)
            {
                var playerIds = team.Players.Select(p => p.Id).ToList();
                
                // Also remove these players from the event participants
                var participants = await _dbcontext.Set<Basketball3x3EventParticipant>()
                    .Where(p => p.EventId == team.EventId && playerIds.Contains(p.PlayerId))
                    .ToListAsync(cancellationToken);
                
                if (participants.Any())
                {
                    _dbcontext.Set<Basketball3x3EventParticipant>().RemoveRange(participants);
                }

                foreach (var player in team.Players)
                {
                    player.BasketballTeamId = null;
                }
            }
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        await base.DeleteAsync(id, cancellationToken);
    }

    protected override void UpdateValues(Basketball3x3Team oldEntity, Basketball3x3Team newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
