using PolytechLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Chess;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;

namespace PolytechLeague.Application.Services.Implementations.Chess;

public class ChessTeamService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ChessTeam>> _logger
) :
    AbstractEntityService<ChessTeam>(_dbcontext, _logger),
    IChessTeamService
{
    public override async Task<ChessTeam> CreateAsync(ChessTeam entity, CancellationToken cancellationToken = default)
    {
        if (entity.PlayerIds != null)
        {
            var players = await _dbcontext.Set<Player>()
                .Where(p => entity.PlayerIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            entity.Players = players;
        }

        await _dbcontext.Set<ChessTeam>().AddAsync(entity, cancellationToken);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public override async Task<ChessTeam> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.Set<ChessTeam>()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (entity == null) throw new EntityNotFoundException(nameof(ChessTeam), id);
        return entity;
    }

    public override async Task<IEnumerable<ChessTeam>> GetAllAsync(
        PolytechLeague.Domain.Params.FilterParams? filterParams = null,
        PolytechLeague.Domain.Params.SortParams? sortParams = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbcontext.Set<ChessTeam>()
            .Include(t => t.Players)
            .ToListAsync(cancellationToken);
    }

    public override async Task UpdateAsync(long id, ChessTeam newEntity, CancellationToken cancellationToken = default)
    {
        var oldEntity = await _dbcontext.Set<ChessTeam>()
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

    protected override void UpdateValues(ChessTeam oldEntity, ChessTeam newEntity)
    {
        oldEntity.Name = newEntity.Name;
    }
}
