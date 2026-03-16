using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Checkers;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Checkers;

namespace PolytechLeague.Application.Services.Implementations.Checkers;

public class CheckersEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<CheckersEvent>> _logger
) :
    AbstractEntityService<CheckersEvent>(_dbcontext, _logger),
    ICheckersEventService
{
    protected override void UpdateValues(CheckersEvent oldEntity, CheckersEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TimeControl = newEntity.TimeControl;
        oldEntity.System = newEntity.System;
        oldEntity.Variant = newEntity.Variant;
    }
}
