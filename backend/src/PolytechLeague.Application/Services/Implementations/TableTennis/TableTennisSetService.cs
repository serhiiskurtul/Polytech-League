using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.TableTennis;

public class TableTennisSetService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<TableTennisSet>> _logger
) :
    AbstractEntityService<TableTennisSet>(_dbcontext, _logger),
    ITableTennisSetService
{
    protected override void UpdateValues(TableTennisSet oldEntity, TableTennisSet newEntity)
    {
        oldEntity.MatchId = newEntity.MatchId;
        oldEntity.SetNumber = newEntity.SetNumber;
        oldEntity.Player1Score = newEntity.Player1Score;
        oldEntity.Player2Score = newEntity.Player2Score;
    }
}
