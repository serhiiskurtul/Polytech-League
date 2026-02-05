using CampusLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using CampusLeague.Application.Services.Interfaces.Chess;
using CampusLeague.Persistence.Database;

namespace CampusLeague.Application.Services.Implementations.Chess;

public class ChessPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ChessPlayer>> _logger
) :
    AbstractEntityService<ChessPlayer>(_dbcontext, _logger),
    IChessPlayerService
{
    protected override void UpdateValues(ChessPlayer oldEntity, ChessPlayer newEntity)
    {
        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
        oldEntity.BirthDate = newEntity.BirthDate;
        oldEntity.PhotoUrl = newEntity.PhotoUrl;
        oldEntity.City = newEntity.City;
        oldEntity.Group = newEntity.Group;
        oldEntity.RatingELO = newEntity.RatingELO;
        oldEntity.FideId = newEntity.FideId;
        oldEntity.Title = newEntity.Title;
    }
}
