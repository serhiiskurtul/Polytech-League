using PolytechLeague.Domain.Entities.Checkers;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;

namespace PolytechLeague.Application.Services.Implementations.Checkers;

public class CheckersTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<CheckersTeam>> _logger
) :
    AbstractEntityService<CheckersTeam>(_dbcontext, _logger),
    ICheckersTeamService
{
    public override async Task<CheckersTeam> CreateAsync(CheckersTeam entity, CancellationToken cancellationToken = default)
    {
        if (entity.PlayerIds != null)
        {
            var players = await _dbcontext.Set<Player>()
                .Where(p => entity.PlayerIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            entity.Players = players;
        }

        await _dbcontext.Set<CheckersTeam>().AddAsync(entity, cancellationToken);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public override async Task<CheckersTeam> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.Set<CheckersTeam>()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (entity == null) throw new EntityNotFoundException(nameof(CheckersTeam), id);
        return entity;
    }

    public override async Task<IEnumerable<CheckersTeam>> GetAllAsync(
        PolytechLeague.Domain.Params.FilterParams? filterParams = null,
        PolytechLeague.Domain.Params.SortParams? sortParams = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbcontext.Set<CheckersTeam>()
            .Include(t => t.Players)
            .ToListAsync(cancellationToken);
    }

    public override async Task UpdateAsync(long id, CheckersTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.Set<CheckersTeam>()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        if (oldEntity == null) return;

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

    protected override void UpdateValues(CheckersTeam oldEntity, CheckersTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Points = newEntity.Points;
        oldEntity.Rank = newEntity.Rank;
        oldEntity.TournamentGroupName = newEntity.TournamentGroupName;
        oldEntity.IsGeneralTableIncluded = newEntity.IsGeneralTableIncluded;
    }
}
