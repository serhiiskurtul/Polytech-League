using PolytechLeague.Domain.Entities.Basketball3x3;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Basketball3x3;

public class Basketball3x3EventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<Basketball3x3Event>> _logger
) :
    AbstractEntityService<Basketball3x3Event>(_dbcontext, _logger),
    IBasketball3x3EventService
{
    protected override void UpdateValues(Basketball3x3Event oldEntity, Basketball3x3Event newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
    }
}
