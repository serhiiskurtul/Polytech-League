using PolytechLeague.Domain.Entities.Volleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Volleyball;

public class VolleyballTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<VolleyballTeam>> _logger
) :
    AbstractEntityService<VolleyballTeam>(_dbcontext, _logger),
    IVolleyballTeamService
{
    protected override void UpdateValues(VolleyballTeam oldEntity, VolleyballTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
