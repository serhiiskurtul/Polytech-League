using PolytechLeague.Domain.Entities.BeachVolleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.BeachVolleyball;

public class BeachVolleyballMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BeachVolleyballMatch>> _logger
) :
    AbstractEntityService<BeachVolleyballMatch>(_dbcontext, _logger),
    IBeachVolleyballMatchService
{
    protected override void UpdateValues(BeachVolleyballMatch oldEntity, BeachVolleyballMatch newEntity)
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
