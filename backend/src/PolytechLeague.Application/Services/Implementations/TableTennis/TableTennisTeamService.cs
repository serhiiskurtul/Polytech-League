using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;

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

    protected override void UpdateValues(TableTennisTeam oldEntity, TableTennisTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Points = newEntity.Points;
        oldEntity.Rank = newEntity.Rank;

        if (newEntity.PlayerIds != null)
        {
            // Explicitly load the collection to ensure EF knows about existing relationships
            _dbcontext.Entry(oldEntity).Collection(t => t.Players!).Load();
            
            var players = _dbcontext.Players
                .Where(p => newEntity.PlayerIds.Contains(p.Id))
                .ToList();
            
            if (oldEntity.Players == null)
            {
                oldEntity.Players = new List<Player>();
            }

            // Remove players not in the new list
            var toRemove = oldEntity.Players.Where(p => !newEntity.PlayerIds.Contains(p.Id)).ToList();
            foreach (var p in toRemove)
            {
                oldEntity.Players.Remove(p);
            }

            // Add new players not already in the collection
            foreach (var p in players)
            {
                if (oldEntity.Players.All(ep => ep.Id != p.Id))
                {
                    oldEntity.Players.Add(p);
                }
            }
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
