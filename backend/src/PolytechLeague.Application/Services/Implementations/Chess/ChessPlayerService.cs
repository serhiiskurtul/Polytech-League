using PolytechLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces.Chess;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Chess;

public class ChessPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ChessPlayer>> _logger
) :
    AbstractEntityService<ChessPlayer>(_dbcontext, _logger),
    IChessPlayerService
{
    protected override void UpdateValues(ChessPlayer oldEntity, ChessPlayer newEntity)
    {
        oldEntity.UserId = newEntity.UserId;
        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
        oldEntity.BirthDate = newEntity.BirthDate;
        oldEntity.PhotoUrl = newEntity.PhotoUrl;
        oldEntity.City = newEntity.City;
        oldEntity.Group = newEntity.Group;
        oldEntity.RatingELO = newEntity.RatingELO;
        oldEntity.FideId = newEntity.FideId;
        oldEntity.Title = newEntity.Title;
    }

    public override async Task<ChessPlayer> CreateAsync(ChessPlayer entity, CancellationToken cancellationToken = default)
    {
        if (entity.UserId.HasValue)
        {
            var existingPlayer = await _dbcontext.Set<ChessPlayer>()
                .FirstOrDefaultAsync(p => p.UserId == entity.UserId, cancellationToken);
            if (existingPlayer != null)
            {
                return existingPlayer;
            }
        }
        return await base.CreateAsync(entity, cancellationToken);
    }
}
