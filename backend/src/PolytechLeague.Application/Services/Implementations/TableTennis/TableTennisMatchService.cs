using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.TableTennis;

public class TableTennisMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<TableTennisMatch>> _logger
) :
    AbstractEntityService<TableTennisMatch>(_dbcontext, _logger),
    ITableTennisMatchService
{
    protected override void UpdateValues(TableTennisMatch oldEntity, TableTennisMatch newEntity)
    {
        oldEntity.TournamentId = newEntity.TournamentId;
        oldEntity.Player1Id = newEntity.Player1Id;
        oldEntity.Player2Id = newEntity.Player2Id;
        oldEntity.WinnerId = newEntity.WinnerId;
        oldEntity.Stage = newEntity.Stage;
        oldEntity.TableNumber = newEntity.TableNumber;
        oldEntity.SetsToWin = newEntity.SetsToWin;
    }
}
