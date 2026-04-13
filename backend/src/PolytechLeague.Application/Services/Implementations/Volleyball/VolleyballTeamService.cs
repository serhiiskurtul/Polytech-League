using PolytechLeague.Domain.Entities.Volleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.Volleyball;

public class VolleyballTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<VolleyballTeam>> _logger
) :
    AbstractEntityService<VolleyballTeam>(_dbcontext, _logger),
    IVolleyballTeamService
{
    public override async Task UpdateAsync(long id, VolleyballTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.Set<VolleyballTeam>()
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

    protected override void UpdateValues(VolleyballTeam oldEntity, VolleyballTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
