using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Checkers;

using Microsoft.EntityFrameworkCore;
public class CheckersGameService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<CheckersGame>> _logger
) :
    AbstractEntityService<CheckersGame>(_dbcontext, _logger),
    ICheckersGameService
{
    protected override void UpdateValues(CheckersGame oldEntity, CheckersGame newEntity)
    {
        oldEntity.TournamentId = newEntity.TournamentId;
        oldEntity.WhitePlayerId = newEntity.WhitePlayerId;
        oldEntity.BlackPlayerId = newEntity.BlackPlayerId;
        oldEntity.Result = newEntity.Result;
        oldEntity.Round = newEntity.Round;
        oldEntity.ScheduledAt = newEntity.ScheduledAt;
        oldEntity.FinishedAt = newEntity.FinishedAt;
        oldEntity.WhiteTeamId = newEntity.WhiteTeamId;
        oldEntity.BlackTeamId = newEntity.BlackTeamId;
        oldEntity.WhiteTeamScore = newEntity.WhiteTeamScore.HasValue ? Math.Min(2.0, newEntity.WhiteTeamScore.Value) : null;
        oldEntity.BlackTeamScore = newEntity.BlackTeamScore.HasValue ? Math.Min(2.0, newEntity.BlackTeamScore.Value) : null;
    }

    public override async Task<CheckersGame> CreateAsync(CheckersGame entity, CancellationToken cancellationToken = default)
    {
        await ResolveParticipantIdsAsync(entity, cancellationToken);
        return await base.CreateAsync(entity, cancellationToken);
    }

    public override async Task UpdateAsync(long id, CheckersGame newEntity, CancellationToken cancellationToken = default)
    {
        await ResolveParticipantIdsAsync(newEntity, cancellationToken);
        await base.UpdateAsync(id, newEntity, cancellationToken);
    }

    private async Task ResolveParticipantIdsAsync(CheckersGame entity, CancellationToken cancellationToken)
    {
        if (entity.WhitePlayerId.HasValue)
        {
            var whiteParticipant = await _dbcontext.Set<CheckersEventParticipant>()
                .FirstOrDefaultAsync(p => p.TournamentId == entity.TournamentId && p.PlayerId == entity.WhitePlayerId.Value, cancellationToken);
            if (whiteParticipant != null)
                entity.WhitePlayerId = whiteParticipant.Id;
        }

        if (entity.BlackPlayerId.HasValue)
        {
            var blackParticipant = await _dbcontext.Set<CheckersEventParticipant>()
                .FirstOrDefaultAsync(p => p.TournamentId == entity.TournamentId && p.PlayerId == entity.BlackPlayerId.Value, cancellationToken);
            if (blackParticipant != null)
                entity.BlackPlayerId = blackParticipant.Id;
        }
    }
}
