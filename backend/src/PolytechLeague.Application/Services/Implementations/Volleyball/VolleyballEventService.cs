using PolytechLeague.Domain.Entities.Volleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Volleyball;

public class VolleyballEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<VolleyballEvent>> _logger
) :
    AbstractEntityService<VolleyballEvent>(_dbcontext, _logger),
    IVolleyballEventService
{
    protected override void UpdateValues(VolleyballEvent oldEntity, VolleyballEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
    }
}
