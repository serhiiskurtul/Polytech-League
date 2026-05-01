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
        var teamInfo = await _dbcontext.BeachVolleyballTeams
            .AsNoTracking()
            .Select(t => new { t.Id, t.EventId, PlayerIds = t.Players!.Select(p => p.Id).ToList() })
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (teamInfo != null && teamInfo.PlayerIds.Any())
        {
            // 1. Bulk delete associated participants
            await _dbcontext.BeachVolleyballEventParticipants
                .Where(p => p.EventId == teamInfo.EventId && teamInfo.PlayerIds.Contains(p.PlayerId))
                .ExecuteDeleteAsync(cancellationToken);

            // 2. Clear BeachVolleyballTeamId for all players in this team
            await _dbcontext.Players
                .Where(p => p.BeachVolleyballTeamId == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.BeachVolleyballTeamId, (long?)null), cancellationToken);
        }

        await base.DeleteAsync(id, cancellationToken);
    }

    protected override void UpdateValues(BeachVolleyballTeam oldEntity, BeachVolleyballTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
