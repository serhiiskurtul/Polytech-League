using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Checkers;

public class CheckersGameService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<CheckersGame>> _logger
) :
    AbstractEntityService<CheckersGame>(_dbcontext, _logger),
    ICheckersGameService
{
    protected override void UpdateValues(CheckersGame oldEntity, CheckersGame newEntity)
    {
        oldEntity.TournamentId = newEntity.TournamentId;
        oldEntity.WhitePlayerId = newEntity.WhitePlayerId;
        oldEntity.BlackPlayerId = newEntity.BlackPlayerId;
        oldEntity.Result = newEntity.Result;
        oldEntity.Round = newEntity.Round;
        oldEntity.ScheduledAt = newEntity.ScheduledAt;
        oldEntity.FinishedAt = newEntity.FinishedAt;
        oldEntity.WhiteTeamId = newEntity.WhiteTeamId;
        oldEntity.BlackTeamId = newEntity.BlackTeamId;
        oldEntity.WhiteTeamScore = newEntity.WhiteTeamScore;
        oldEntity.BlackTeamScore = newEntity.BlackTeamScore;
    }
}
