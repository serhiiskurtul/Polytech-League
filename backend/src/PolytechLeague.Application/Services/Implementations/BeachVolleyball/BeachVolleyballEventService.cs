using PolytechLeague.Domain.Entities.BeachVolleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.BeachVolleyball;

public class BeachVolleyballEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BeachVolleyballEvent>> _logger
) :
    AbstractEntityService<BeachVolleyballEvent>(_dbcontext, _logger),
    IBeachVolleyballEventService
{
    protected override void UpdateValues(BeachVolleyballEvent oldEntity, BeachVolleyballEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
    }
}
