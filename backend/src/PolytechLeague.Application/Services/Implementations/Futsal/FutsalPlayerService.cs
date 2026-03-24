using PolytechLeague.Domain.Entities.Futsal;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Futsal;

public class FutsalPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<FutsalPlayer>> _logger
) :
    AbstractEntityService<FutsalPlayer>(_dbcontext, _logger),
    IFutsalPlayerService
{
    protected override void UpdateValues(FutsalPlayer oldEntity, FutsalPlayer newEntity)
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
