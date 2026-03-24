using PolytechLeague.Domain.Entities.Futsal;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Futsal;

public class FutsalMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<FutsalMatch>> _logger
) :
    AbstractEntityService<FutsalMatch>(_dbcontext, _logger),
    IFutsalMatchService
{
    protected override void UpdateValues(FutsalMatch oldEntity, FutsalMatch newEntity)
    {
        oldEntity.EventId = newEntity.EventId;
        oldEntity.HomeTeamId = newEntity.HomeTeamId;
        oldEntity.AwayTeamId = newEntity.AwayTeamId;
        oldEntity.HomeTeamScore = newEntity.HomeTeamScore;
        oldEntity.AwayTeamScore = newEntity.AwayTeamScore;
        oldEntity.WinnerTeamId = newEntity.WinnerTeamId;
        oldEntity.Stage = newEntity.Stage;
    }
}
