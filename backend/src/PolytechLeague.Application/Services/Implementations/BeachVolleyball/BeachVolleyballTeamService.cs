using PolytechLeague.Domain.Entities.BeachVolleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.BeachVolleyball;

public class BeachVolleyballTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BeachVolleyballTeam>> _logger
) :
    AbstractEntityService<BeachVolleyballTeam>(_dbcontext, _logger),
    IBeachVolleyballTeamService
{
    protected override void UpdateValues(BeachVolleyballTeam oldEntity, BeachVolleyballTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
