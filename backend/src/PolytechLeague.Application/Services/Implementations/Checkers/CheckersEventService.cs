using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Application.Dto.CheckersDto;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Exceptions;

namespace PolytechLeague.Application.Services.Implementations.Checkers;

public class CheckersEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<CheckersEvent>> _logger
) :
    AbstractEntityService<CheckersEvent>(_dbcontext, _logger),
    ICheckersEventService
{
    protected override void UpdateValues(CheckersEvent oldEntity, CheckersEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TimeControl = newEntity.TimeControl;
        oldEntity.System = newEntity.System;
        oldEntity.Variant = newEntity.Variant;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.Schedule = newEntity.Schedule;
        oldEntity.RefereeIds = newEntity.RefereeIds;
        oldEntity.FirstPlaceId = newEntity.FirstPlaceId;
        oldEntity.SecondPlaceId = newEntity.SecondPlaceId;
        oldEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
    }

    public override async Task<CheckersEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.Set<CheckersEvent>()
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .Include(e => e.Games!)
                .ThenInclude(g => g.WhitePlayer!)
                    .ThenInclude(p => p.Player)
            .Include(e => e.Games!)
                .ThenInclude(g => g.BlackPlayer!)
                    .ThenInclude(p => p.Player)
            .Include(e => e.Photos)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(CheckersEvent), id);
        }

        return entity;
    }

    public async Task AddParticipantAsync(long id, CheckersEventParticipantDto request, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<CheckersEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == id && p.PlayerId == request.PlayerId, cancellationToken);
        
        if (participant != null) return;

        var entity = request.FromDto(id);
        await _dbcontext.Set<CheckersEventParticipant>().AddAsync(entity, cancellationToken);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateParticipantAsync(long id, long playerId, CheckersEventParticipantDto request, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<CheckersEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == id && p.PlayerId == playerId, cancellationToken);

        if (participant == null) throw new EntityNotFoundException("Учасника не знайдено", playerId);

        participant.Points = request.Points;
        participant.Rank = request.Rank;
        _dbcontext.Set<CheckersEventParticipant>().Update(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveParticipantAsync(long id, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<CheckersEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == id && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            _dbcontext.Set<CheckersEventParticipant>().Remove(participant);
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
