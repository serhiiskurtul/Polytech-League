using PolytechLeague.Domain.Entities.BeachVolleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.BeachVolleyball;

public class BeachVolleyballPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BeachVolleyballPlayer>> _logger
) :
    AbstractEntityService<BeachVolleyballPlayer>(_dbcontext, _logger),
    IBeachVolleyballPlayerService
{
    protected override void UpdateValues(BeachVolleyballPlayer oldEntity, BeachVolleyballPlayer newEntity)
    {
        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
        oldEntity.BirthDate = newEntity.BirthDate;
        oldEntity.PhotoUrl = newEntity.PhotoUrl;
        oldEntity.City = newEntity.City;
        oldEntity.Group = newEntity.Group;

        oldEntity.TeamId = newEntity.TeamId;
        oldEntity.ShirtNumber = newEntity.ShirtNumber;
    }
}
