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
    public override async Task<TableTennisTeam> CreateAsync(TableTennisTeam entity, CancellationToken cancellationToken = default)
    {
        if (entity.PlayerIds != null)
        {
            var players = await _dbcontext.Players
                .Where(p => entity.PlayerIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            
            entity.Players = players;
        }

        return await base.CreateAsync(entity, cancellationToken);
    }

    public override async Task UpdateAsync(long id, TableTennisTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.TableTennisTeams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        if (oldEntity == null) throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(TableTennisTeam), id);

        UpdateValues(oldEntity, newEntity);

        if (newEntity.PlayerIds != null)
        {
            var players = await _dbcontext.Set<Player>()
                .Where(p => newEntity.PlayerIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            
            oldEntity.Players = players;
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    protected override void UpdateValues(TableTennisTeam oldEntity, TableTennisTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Points = newEntity.Points;
        oldEntity.Rank = newEntity.Rank;
    }

    public override async Task<TableTennisTeam> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.TableTennisTeams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(TableTennisTeam), id);
    }
}
