using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonPlayer>> _logger
) :
    AbstractEntityService<BadmintonPlayer>(_dbcontext, _logger),
    IBadmintonPlayerService
{
    protected override void UpdateValues(BadmintonPlayer oldEntity, BadmintonPlayer newEntity)
    {
        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
    }
}
