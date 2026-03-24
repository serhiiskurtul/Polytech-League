using PolytechLeague.Domain.Entities.Basketball3x3;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Basketball3x3;

public class Basketball3x3PlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<Basketball3x3Player>> _logger
) :
    AbstractEntityService<Basketball3x3Player>(_dbcontext, _logger),
    IBasketball3x3PlayerService
{
    protected override void UpdateValues(Basketball3x3Player oldEntity, Basketball3x3Player newEntity)
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
