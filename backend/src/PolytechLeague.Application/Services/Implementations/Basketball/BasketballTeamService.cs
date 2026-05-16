using PolytechLeague.Domain.Entities.Basketball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.Basketball;

public class BasketballTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BasketballTeam>> _logger
) :
    AbstractEntityService<BasketballTeam>(_dbcontext, _logger),
    IBasketballTeamService
{
    public override async Task UpdateAsync(long id, BasketballTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.Set<BasketballTeam>()
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
        // 1. Get EventId and PlayerIds directly
        var eventId = await _dbcontext.BasketballTeams
            .Where(t => t.Id == id)
            .Select(t => t.EventId)
            .FirstOrDefaultAsync(cancellationToken);

        if (eventId == 0) 
        {
            await base.DeleteAsync(id, cancellationToken);
            return;
        }

        var playerIds = await _dbcontext.Players
            .Where(p => p.BasketballTeamId == id)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (playerIds.Any())
        {
            // 2. Bulk delete associated participants
            await _dbcontext.BasketballEventParticipants
                .Where(p => p.EventId == eventId && playerIds.Contains(p.PlayerId))
                .ExecuteDeleteAsync(cancellationToken);

            // 3. Clear BasketballTeamId for all players in this team
            await _dbcontext.Players
                .Where(p => p.BasketballTeamId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.BasketballTeamId, (long?)null), cancellationToken);
        }

        await base.DeleteAsync(id, cancellationToken);
    }

    protected override void UpdateValues(BasketballTeam oldEntity, BasketballTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
