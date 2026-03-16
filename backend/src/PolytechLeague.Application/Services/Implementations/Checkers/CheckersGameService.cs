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
        oldEntity.CheckersEventId = newEntity.CheckersEventId;
        oldEntity.WhitePlayerId = newEntity.WhitePlayerId;
        oldEntity.BlackPlayerId = newEntity.BlackPlayerId;
        oldEntity.Result = newEntity.Result;
        oldEntity.RoundNumber = newEntity.RoundNumber;
        oldEntity.ScheduledAt = newEntity.ScheduledAt;
        oldEntity.FinishedAt = newEntity.FinishedAt;
    }
}
