using PolytechLeague.Domain.Entities.Armwrestling;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Armwrestling;

public class ArmwrestlingMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ArmwrestlingMatch>> _logger
) :
    AbstractEntityService<ArmwrestlingMatch>(_dbcontext, _logger),
    IArmwrestlingMatchService
{
    protected override void UpdateValues(ArmwrestlingMatch oldEntity, ArmwrestlingMatch newEntity)
    {
        oldEntity.Player1Id = newEntity.Player1Id;
        oldEntity.Player2Id = newEntity.Player2Id;
        oldEntity.WinnerId = newEntity.WinnerId;
        oldEntity.Stage = newEntity.Stage;
    }
}
