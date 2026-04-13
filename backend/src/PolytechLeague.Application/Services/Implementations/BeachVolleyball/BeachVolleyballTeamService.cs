using PolytechLeague.Domain.Entities.BeachVolleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.BeachVolleyball;

public class BeachVolleyballTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BeachVolleyballTeam>> _logger
) :
    AbstractEntityService<BeachVolleyballTeam>(_dbcontext, _logger),
    IBeachVolleyballTeamService
{
    public override async Task UpdateAsync(long id, BeachVolleyballTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.Set<BeachVolleyballTeam>()
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

    protected override void UpdateValues(BeachVolleyballTeam oldEntity, BeachVolleyballTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
