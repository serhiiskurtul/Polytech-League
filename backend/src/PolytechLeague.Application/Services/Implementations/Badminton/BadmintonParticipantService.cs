using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonParticipantService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonEventParticipant>> _logger
) :
    AbstractEntityService<BadmintonEventParticipant>(_dbcontext, _logger),
    IBadmintonParticipantService
{
    protected override void UpdateValues(BadmintonEventParticipant oldEntity, BadmintonEventParticipant newEntity)
    {
        oldEntity.Points = newEntity.Points;
        oldEntity.Rank = newEntity.Rank;
    }
}
