using PolytechLeague.Domain.Entities.Futsal;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Futsal;

public class FutsalEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<FutsalEvent>> _logger
) :
    AbstractEntityService<FutsalEvent>(_dbcontext, _logger),
    IFutsalEventService
{
    protected override void UpdateValues(FutsalEvent oldEntity, FutsalEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
    }
}
