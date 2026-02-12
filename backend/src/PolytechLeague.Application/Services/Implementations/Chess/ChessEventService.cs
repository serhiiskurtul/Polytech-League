using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Chess;

namespace PolytechLeague.Application.Services.Implementations.Chess;

public class ChessEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ChessEvent>> _logger
) :
    AbstractEntityService<ChessEvent>(_dbcontext, _logger),
    IChessEventService
{
    protected override void UpdateValues(ChessEvent oldEntity, ChessEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TimeControl = newEntity.TimeControl;
        oldEntity.System = newEntity.System;
    }
}
