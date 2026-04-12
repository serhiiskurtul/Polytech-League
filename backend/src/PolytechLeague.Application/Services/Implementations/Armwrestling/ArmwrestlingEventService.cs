using PolytechLeague.Domain.Entities.Armwrestling;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Exceptions;

namespace PolytechLeague.Application.Services.Implementations.Armwrestling;

public class ArmwrestlingEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ArmwrestlingEvent>> _logger
) :
    AbstractEntityService<ArmwrestlingEvent>(_dbcontext, _logger),
    IArmwrestlingEventService
{
    public override async Task<ArmwrestlingEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching ArmwrestlingEvent with id {Id} including matches and players", id);

        var entity = await _dbcontext.ArmwrestlingEvents
            .Include(e => e.Photos)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player1)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player2)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            _logger.LogWarning("ArmwrestlingEvent with id {Id} not found", id);
            throw new EntityNotFoundException(nameof(ArmwrestlingEvent), id);
        }

        return entity;
    }

    protected override void UpdateValues(ArmwrestlingEvent oldEntity, ArmwrestlingEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.Hand = newEntity.Hand;
        oldEntity.WeightClass = newEntity.WeightClass;
        oldEntity.Category = newEntity.Category;
    }
}
