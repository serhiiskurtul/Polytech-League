using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonMatch>> _logger
) :
    AbstractEntityService<BadmintonMatch>(_dbcontext, _logger),
    IBadmintonMatchService
{
    protected override void UpdateValues(BadmintonMatch oldEntity, BadmintonMatch newEntity)
    {
        oldEntity.WinnerId = newEntity.WinnerId;
        oldEntity.Stage = newEntity.Stage;
        oldEntity.CourtNumber = newEntity.CourtNumber;
        oldEntity.SetsToWin = newEntity.SetsToWin;
    }
}
