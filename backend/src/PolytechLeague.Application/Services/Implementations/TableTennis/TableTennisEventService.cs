using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.TableTennis;

public class TableTennisEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<TableTennisEvent>> _logger
) :
    AbstractEntityService<TableTennisEvent>(_dbcontext, _logger),
    ITableTennisEventService
{
    protected override void UpdateValues(TableTennisEvent oldEntity, TableTennisEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TournamentType = newEntity.TournamentType;
        oldEntity.Category = newEntity.Category;
        oldEntity.UpdatedAt = DateTime.UtcNow;
    }
}
