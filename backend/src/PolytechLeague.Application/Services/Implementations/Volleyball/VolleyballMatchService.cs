using PolytechLeague.Domain.Entities.Volleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Volleyball;

public class VolleyballMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<VolleyballMatch>> _logger
) :
    AbstractEntityService<VolleyballMatch>(_dbcontext, _logger),
    IVolleyballMatchService
{
    protected override void UpdateValues(VolleyballMatch oldEntity, VolleyballMatch newEntity)
    {
        oldEntity.EventId = newEntity.EventId;
        oldEntity.HomeTeamId = newEntity.HomeTeamId;
        oldEntity.AwayTeamId = newEntity.AwayTeamId;
        oldEntity.SetsToWin = newEntity.SetsToWin;
        oldEntity.HomeSetsWon = newEntity.HomeSetsWon;
        oldEntity.AwaySetsWon = newEntity.AwaySetsWon;
        oldEntity.WinnerTeamId = newEntity.WinnerTeamId;
        oldEntity.Stage = newEntity.Stage;
    }
}
