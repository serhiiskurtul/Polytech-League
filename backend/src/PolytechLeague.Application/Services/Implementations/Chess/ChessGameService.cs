using PolytechLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Chess;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Chess;

public class ChessGameService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ChessGame>> _logger
) :
    AbstractEntityService<ChessGame>(_dbcontext, _logger),
    IChessGameService
{
    protected override void UpdateValues(ChessGame oldEntity, ChessGame newEntity)
    {
        oldEntity.TournamentId = newEntity.TournamentId;
        oldEntity.WhitePlayerId = newEntity.WhitePlayerId;
        oldEntity.BlackPlayerId = newEntity.BlackPlayerId;
        oldEntity.Result = newEntity.Result;
        oldEntity.Round = newEntity.Round;
        oldEntity.Board = newEntity.Board;
    }
}
