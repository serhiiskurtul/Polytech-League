using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.TableTennis;

public class TableTennisEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<TableTennisEvent>> _logger
) :
    AbstractEntityService<TableTennisEvent>(_dbcontext, _logger),
    ITableTennisEventService
{
    protected override void UpdateValues(TableTennisEvent oldEntity, TableTennisEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TournamentType = newEntity.TournamentType;
        oldEntity.Category = newEntity.Category;
        oldEntity.FirstPlaceId = newEntity.FirstPlaceId;
        oldEntity.SecondPlaceId = newEntity.SecondPlaceId;
        oldEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
        oldEntity.UpdatedAt = DateTime.UtcNow;
    }

    public override async Task<TableTennisEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.TableTennisEvents
            .Include(e => e.Photos)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player1)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player2)
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(TableTennisEvent), id);
    }
}
