using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Chess;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Exceptions;

namespace PolytechLeague.Application.Services.Implementations.Chess;

public class ChessEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ChessEvent>> _logger
) :
    AbstractEntityService<ChessEvent>(_dbcontext, _logger),
    IChessEventService
{
    protected override void UpdateValues(ChessEvent oldEntity, ChessEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TimeControl = newEntity.TimeControl;
        oldEntity.System = newEntity.System;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.RefereeIds = newEntity.RefereeIds;
        oldEntity.Schedule = newEntity.Schedule;
    }

    public override async Task<ChessEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.Set<ChessEvent>()
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .Include(e => e.Games!)
                .ThenInclude(g => g.WhitePlayer)
            .Include(e => e.Games!)
                .ThenInclude(g => g.BlackPlayer)
            .Include(e => e.Photos)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(ChessEvent), id);
        }

        return entity;
    }

    public async Task AddParticipantAsync(long eventId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _dbcontext.Set<ChessEventParticipant>()
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == dto.PlayerId, cancellationToken);

        if (exists) return;

        var participant = new ChessEventParticipant
        {
            TournamentId = eventId,
            PlayerId = dto.PlayerId,
            Points = dto.Points,
            Rank = dto.Rank
        };
        _dbcontext.Set<ChessEventParticipant>().Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateParticipantAsync(long eventId, long playerId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<ChessEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            participant.Points = dto.Points;
            participant.Rank = dto.Rank;
            // Removed Buchholz and SonnebornBerger from Entity since they are not mapped
            _dbcontext.Set<ChessEventParticipant>().Update(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<ChessEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            _dbcontext.Set<ChessEventParticipant>().Remove(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AddPhotoAsync(long eventId, string url, CancellationToken cancellationToken = default)
    {
        var photo = new PolytechLeague.Domain.Entities.EventPhotos
        {
            EventId = eventId,
            Url = url
        };
        _dbcontext.Set<PolytechLeague.Domain.Entities.EventPhotos>().Add(photo);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePhotoAsync(long eventId, long photoId, CancellationToken cancellationToken = default)
    {
        var photo = await _dbcontext.Set<PolytechLeague.Domain.Entities.EventPhotos>()
            .FirstOrDefaultAsync(p => p.Id == photoId && p.EventId == eventId, cancellationToken);
        if (photo != null)
        {
            _dbcontext.Set<PolytechLeague.Domain.Entities.EventPhotos>().Remove(photo);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }
}
