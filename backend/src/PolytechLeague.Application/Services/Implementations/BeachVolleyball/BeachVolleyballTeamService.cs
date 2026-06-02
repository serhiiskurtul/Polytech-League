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
    public override async Task<BeachVolleyballTeam> CreateAsync(BeachVolleyballTeam entity, CancellationToken cancellationToken = default)
    {
        if (entity.PlayerIds != null)
        {
            var players = await _dbcontext.Set<Player>()
                .Where(p => entity.PlayerIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            
            entity.Players = players;
        }

        return await base.CreateAsync(entity, cancellationToken);
    }

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

    public override async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        // 1. Get EventId and PlayerIds directly
        var eventId = await _dbcontext.BeachVolleyballTeams
            .Where(t => t.Id == id)
            .Select(t => t.EventId)
            .FirstOrDefaultAsync(cancellationToken);

        if (eventId == 0) 
        {
            await base.DeleteAsync(id, cancellationToken);
            return;
        }

        var teamWithPlayers = await _dbcontext.BeachVolleyballTeams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (teamWithPlayers?.Players != null && teamWithPlayers.Players.Any())
        {
            var playerIds = teamWithPlayers.Players.Select(p => p.Id).ToList();
            
            // 2. Bulk delete associated participants
            await _dbcontext.BeachVolleyballEventParticipants
                .Where(p => p.EventId == eventId && playerIds.Contains(p.PlayerId))
                .ExecuteDeleteAsync(cancellationToken);
                
            // 3. Clear team from players (EF Core will delete the join table rows)
            teamWithPlayers.Players.Clear();
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        await base.DeleteAsync(id, cancellationToken);
    }

    protected override void UpdateValues(BeachVolleyballTeam oldEntity, BeachVolleyballTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
