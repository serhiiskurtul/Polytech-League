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
        // 1. Bulk delete associated participants using a subquery
        await _dbcontext.Basketball3x3EventParticipants
            .Where(p => p.EventId == _dbcontext.Basketball3x3Teams
                .Where(t => t.Id == id)
                .Select(t => t.EventId)
                .FirstOrDefault() &&
                _dbcontext.Players.Any(pl => pl.Id == p.PlayerId && pl.BasketballTeamId == id))
            .ExecuteDeleteAsync(cancellationToken);

        // 2. Clear BasketballTeamId for all players in this team
        await _dbcontext.Players
            .Where(p => p.BasketballTeamId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.BasketballTeamId, (long?)null), cancellationToken);

        await base.DeleteAsync(id, cancellationToken);
    }

    protected override void UpdateValues(Basketball3x3Team oldEntity, Basketball3x3Team newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
