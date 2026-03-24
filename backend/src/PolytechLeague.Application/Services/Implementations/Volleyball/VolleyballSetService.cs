using PolytechLeague.Domain.Entities.Volleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Volleyball;

public class VolleyballSetService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<VolleyballSet>> _logger
) :
    AbstractEntityService<VolleyballSet>(_dbcontext, _logger),
    IVolleyballSetService
{
    protected override void UpdateValues(VolleyballSet oldEntity, VolleyballSet newEntity)
    {
        oldEntity.MatchId = newEntity.MatchId;
        oldEntity.SetNumber = newEntity.SetNumber;
        oldEntity.HomeScore = newEntity.HomeScore;
        oldEntity.AwayScore = newEntity.AwayScore;
    }
}
