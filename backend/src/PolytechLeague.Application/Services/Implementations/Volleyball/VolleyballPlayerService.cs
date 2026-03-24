using PolytechLeague.Domain.Entities.Volleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Volleyball;

public class VolleyballPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<VolleyballPlayer>> _logger
) :
    AbstractEntityService<VolleyballPlayer>(_dbcontext, _logger),
    IVolleyballPlayerService
{
    protected override void UpdateValues(VolleyballPlayer oldEntity, VolleyballPlayer newEntity)
    {
        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
        oldEntity.BirthDate = newEntity.BirthDate;
        oldEntity.PhotoUrl = newEntity.PhotoUrl;
        oldEntity.City = newEntity.City;
        oldEntity.Group = newEntity.Group;

        oldEntity.TeamId = newEntity.TeamId;
        oldEntity.Position = newEntity.Position;
        oldEntity.ShirtNumber = newEntity.ShirtNumber;
    }
}
