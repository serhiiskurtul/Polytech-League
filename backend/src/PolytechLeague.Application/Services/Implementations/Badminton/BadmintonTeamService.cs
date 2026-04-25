using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonTeamService(
    ApplicationDbContext context,
    ILogger<AbstractEntityService<BadmintonTeam>> logger
) :
    AbstractEntityService<BadmintonTeam>(context, logger),
    IBadmintonTeamService
{
    public override async Task<BadmintonTeam> CreateAsync(BadmintonTeam entity, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Creating badminton team with player mapping.");
        
        if (entity.PlayerIds != null && entity.PlayerIds.Count >= 2)
        {
            var ids = entity.PlayerIds.ToList();
            entity.Player1Id = ids[0];
            entity.Player2Id = ids[1];
        }

        context.Set<BadmintonTeam>().Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return entity;
    }

    public override async Task UpdateAsync(long id, BadmintonTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await context.Set<BadmintonTeam>()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        if (oldEntity == null) return;

        UpdateValues(oldEntity, newEntity);

        if (newEntity.PlayerIds != null && newEntity.PlayerIds.Count >= 2)
        {
            var ids = newEntity.PlayerIds.ToList();
            oldEntity.Player1Id = ids[0];
            oldEntity.Player2Id = ids[1];
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    protected override void UpdateValues(BadmintonTeam oldEntity, BadmintonTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Player1Id = newEntity.Player1Id;
        oldEntity.Player2Id = newEntity.Player2Id;
        if (newEntity.EventId.HasValue)
        {
            oldEntity.EventId = newEntity.EventId;
        }
    }
}
