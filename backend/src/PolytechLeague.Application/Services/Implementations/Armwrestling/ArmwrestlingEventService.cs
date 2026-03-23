using PolytechLeague.Domain.Entities.Armwrestling;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Armwrestling;

public class ArmwrestlingEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ArmwrestlingEvent>> _logger
) :
    AbstractEntityService<ArmwrestlingEvent>(_dbcontext, _logger),
    IArmwrestlingEventService
{
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
