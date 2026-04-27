using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.Application.Services.Implementations.TableTennis;

public class TableTennisTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<TableTennisTeam>> _logger,
    INotificationService _notificationService
) :
    AbstractEntityService<TableTennisTeam>(_dbcontext, _logger, _notificationService),
    ITableTennisTeamService
{
    protected override void UpdateValues(TableTennisTeam oldEntity, TableTennisTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Points = newEntity.Points;
        oldEntity.Rank = newEntity.Rank;

        if (newEntity.PlayerIds != null)
        {
            var existingPlayers = _dbcontext.Players
                .Where(p => newEntity.PlayerIds.Contains(p.Id))
                .ToList();
            oldEntity.Players = existingPlayers;
        }
    }

    public override async Task<TableTennisTeam> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.TableTennisTeams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(TableTennisTeam), id);
    }
}
