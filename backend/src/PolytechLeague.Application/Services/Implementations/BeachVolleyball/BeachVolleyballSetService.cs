using PolytechLeague.Domain.Entities.BeachVolleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.BeachVolleyball;

public class BeachVolleyballSetService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BeachVolleyballSet>> _logger
) :
    AbstractEntityService<BeachVolleyballSet>(_dbcontext, _logger),
    IBeachVolleyballSetService
{
    protected override void UpdateValues(BeachVolleyballSet oldEntity, BeachVolleyballSet newEntity)
    {
        oldEntity.MatchId = newEntity.MatchId;
        oldEntity.SetNumber = newEntity.SetNumber;
        oldEntity.HomeScore = newEntity.HomeScore;
        oldEntity.AwayScore = newEntity.AwayScore;
    }
}
