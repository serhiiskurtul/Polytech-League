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
    public override async Task<VolleyballTeam> CreateAsync(VolleyballTeam entity, CancellationToken cancellationToken = default)
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

    public override async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        // 1. Get EventId and PlayerIds directly
        var eventId = await _dbcontext.VolleyballTeams
            .Where(t => t.Id == id)
            .Select(t => t.EventId)
            .FirstOrDefaultAsync(cancellationToken);

        if (eventId == 0) 
        {
            await base.DeleteAsync(id, cancellationToken);
            return;
        }

        var teamWithPlayers = await _dbcontext.VolleyballTeams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (teamWithPlayers?.Players != null && teamWithPlayers.Players.Any())
        {
            var playerIds = teamWithPlayers.Players.Select(p => p.Id).ToList();
            
            // 2. Bulk delete associated participants
            await _dbcontext.VolleyballEventParticipants
                .Where(p => p.EventId == eventId && playerIds.Contains(p.PlayerId))
                .ExecuteDeleteAsync(cancellationToken);
                
            // 3. Clear team from players (EF Core will delete the join table rows)
            teamWithPlayers.Players.Clear();
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        await base.DeleteAsync(id, cancellationToken);
    }

    protected override void UpdateValues(VolleyballTeam oldEntity, VolleyballTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Rank = newEntity.Rank;
        oldEntity.TournamentGroupName = newEntity.TournamentGroupName;
        oldEntity.IsGeneralTableIncluded = newEntity.IsGeneralTableIncluded;
    }
}
