using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonTeam>> _logger
) :
    AbstractEntityService<BadmintonTeam>(_dbcontext, _logger),
    IBadmintonTeamService
{
    public override async Task UpdateAsync(long id, BadmintonTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.Set<BadmintonTeam>()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        if (oldEntity == null) return;

        UpdateValues(oldEntity, newEntity);

        if (newEntity.PlayerIds != null && newEntity.PlayerIds.Count >= 2)
        {
            var ids = newEntity.PlayerIds.ToList();
            oldEntity.Player1Id = ids[0];
            oldEntity.Player2Id = ids[1];
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
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
