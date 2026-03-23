using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.TableTennis;

public class TableTennisPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<TableTennisPlayer>> _logger
) :
    AbstractEntityService<TableTennisPlayer>(_dbcontext, _logger),
    ITableTennisPlayerService
{
    protected override void UpdateValues(TableTennisPlayer oldEntity, TableTennisPlayer newEntity)
    {
        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
        oldEntity.BirthDate = newEntity.BirthDate;
        oldEntity.PhotoUrl = newEntity.PhotoUrl;
        oldEntity.City = newEntity.City;
        oldEntity.Group = newEntity.Group;
        oldEntity.Rating = newEntity.Rating;
        oldEntity.PlayingHand = newEntity.PlayingHand;
    }
}
