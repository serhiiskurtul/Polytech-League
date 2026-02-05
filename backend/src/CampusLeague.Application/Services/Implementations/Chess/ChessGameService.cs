using CampusLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using CampusLeague.Application.Services.Interfaces.Chess;
using CampusLeague.Persistence.Database;

namespace CampusLeague.Application.Services.Implementations.Chess;

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
