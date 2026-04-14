using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Extensions;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonMatchService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonMatch>> _logger
) :
    AbstractEntityService<BadmintonMatch>(_dbcontext, _logger),
    IBadmintonMatchService
{
    public override async Task<IEnumerable<BadmintonMatch>> GetAllAsync(
        PolytechLeague.Domain.Params.FilterParams? filterParams = null,
        PolytechLeague.Domain.Params.SortParams? sortParams = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbcontext.BadmintonMatches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Sets)
            .Filter(filterParams)
            .Sort(sortParams)
            .ToListAsync(cancellationToken);
    }

    public override async Task<BadmintonMatch> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.BadmintonMatches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Sets)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (entity == null)
        {
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(BadmintonMatch), id);
        }

        return entity;
    }

    public override async Task<BadmintonMatch> CreateAsync(BadmintonMatch entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating badminton match.");
        
        _dbcontext.BadmintonMatches.Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        
        return entity;
    }

    protected override void UpdateValues(BadmintonMatch oldEntity, BadmintonMatch newEntity)
    {
        oldEntity.HomeScore = newEntity.HomeScore;
        oldEntity.AwayScore = newEntity.AwayScore;
        oldEntity.WinnerPlayerId = newEntity.WinnerPlayerId;
        oldEntity.WinnerTeamId = newEntity.WinnerTeamId;
        oldEntity.Stage = newEntity.Stage;
        oldEntity.CourtNumber = newEntity.CourtNumber;
        oldEntity.SetsToWin = newEntity.SetsToWin;
    }
}
