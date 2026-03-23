using PolytechLeague.Domain.Entities.Armwrestling;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Armwrestling;

public class ArmwrestlingPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ArmwrestlingPlayer>> _logger
) :
    AbstractEntityService<ArmwrestlingPlayer>(_dbcontext, _logger),
    IArmwrestlingPlayerService
{
    protected override void UpdateValues(ArmwrestlingPlayer oldEntity, ArmwrestlingPlayer newEntity)
    {
        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
        oldEntity.BirthDate = newEntity.BirthDate;
        oldEntity.PhotoUrl = newEntity.PhotoUrl;
        oldEntity.City = newEntity.City;
        oldEntity.Group = newEntity.Group;
        oldEntity.CurrentWeight = newEntity.CurrentWeight;
        oldEntity.PreferredWeightClass = newEntity.PreferredWeightClass;
    }
}
