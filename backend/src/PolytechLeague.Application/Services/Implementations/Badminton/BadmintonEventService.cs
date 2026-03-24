using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonEvent>> _logger
) :
    AbstractEntityService<BadmintonEvent>(_dbcontext, _logger),
    IBadmintonEventService
{
    protected override void UpdateValues(BadmintonEvent oldEntity, BadmintonEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TournamentType = newEntity.TournamentType;
    }
}
