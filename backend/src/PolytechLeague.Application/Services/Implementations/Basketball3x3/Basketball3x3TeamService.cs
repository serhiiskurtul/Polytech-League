using PolytechLeague.Domain.Entities.Basketball3x3;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Basketball3x3;

public class Basketball3x3TeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<Basketball3x3Team>> _logger
) :
    AbstractEntityService<Basketball3x3Team>(_dbcontext, _logger),
    IBasketball3x3TeamService
{
    protected override void UpdateValues(Basketball3x3Team oldEntity, Basketball3x3Team newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
