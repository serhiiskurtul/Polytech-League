using PolytechLeague.Domain.Entities.Armwrestling;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Params;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Exceptions;

namespace PolytechLeague.Application.Services.Implementations.Armwrestling;

public class ArmwrestlingMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ArmwrestlingMatch>> _logger
) :
    AbstractEntityService<ArmwrestlingMatch>(_dbcontext, _logger),
    IArmwrestlingMatchService
{
    public override async Task<IEnumerable<ArmwrestlingMatch>> GetAllAsync(
        FilterParams? filterParams = null,
        SortParams? sortParams = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbcontext.ArmwrestlingMatches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .ToListAsync(cancellationToken);
    }

    public override async Task<ArmwrestlingMatch> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.ArmwrestlingMatches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (entity is null) throw new EntityNotFoundException(nameof(ArmwrestlingMatch), id);
        return entity;
    }

    protected override void UpdateValues(ArmwrestlingMatch oldEntity, ArmwrestlingMatch newEntity)
    {
        if (newEntity.Player1Id > 0) oldEntity.Player1Id = newEntity.Player1Id;
        if (newEntity.Player2Id > 0) oldEntity.Player2Id = newEntity.Player2Id;
        oldEntity.WinnerId = newEntity.WinnerId;
        oldEntity.Stage = newEntity.Stage;
        if (newEntity.TableNumber > 0) oldEntity.TableNumber = newEntity.TableNumber;
    }
}
