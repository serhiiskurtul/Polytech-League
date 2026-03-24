using PolytechLeague.Domain.Entities.Basketball3x3;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Basketball3x3;

public class Basketball3x3MatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<Basketball3x3Match>> _logger
) :
    AbstractEntityService<Basketball3x3Match>(_dbcontext, _logger),
    IBasketball3x3MatchService
{
    protected override void UpdateValues(Basketball3x3Match oldEntity, Basketball3x3Match newEntity)
    {
        oldEntity.EventId = newEntity.EventId;
        oldEntity.HomeTeamId = newEntity.HomeTeamId;
        oldEntity.AwayTeamId = newEntity.AwayTeamId;
        oldEntity.HomeTeamScore = newEntity.HomeTeamScore;
        oldEntity.AwayTeamScore = newEntity.AwayTeamScore;
        oldEntity.WinnerTeamId = newEntity.WinnerTeamId;
        oldEntity.Stage = newEntity.Stage;
    }
}
