using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;

using PolytechLeague.Application.Dto.BadmintonDto;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonEvent>> _logger,
    INotificationService _notificationService
) :
    AbstractEntityService<BadmintonEvent>(_dbcontext, _logger, _notificationService),
    IBadmintonEventService
{
    protected override void UpdateValues(BadmintonEvent oldEntity, BadmintonEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.RegistrationStart = newEntity.RegistrationStart;
        oldEntity.RegistrationEnd = newEntity.RegistrationEnd;
        oldEntity.EndDate = newEntity.EndDate;
        oldEntity.Level = newEntity.Level;
        oldEntity.TournamentType = newEntity.TournamentType;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.Schedule = newEntity.Schedule;
        oldEntity.FirstPlaceId = newEntity.FirstPlaceId;
        oldEntity.SecondPlaceId = newEntity.SecondPlaceId;
        oldEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
        oldEntity.FirstPlaceTeamId = newEntity.FirstPlaceTeamId;
        oldEntity.SecondPlaceTeamId = newEntity.SecondPlaceTeamId;
        oldEntity.ThirdPlaceTeamId = newEntity.ThirdPlaceTeamId;
        oldEntity.IsTeam = newEntity.IsTeam;
    }

    public override async Task<BadmintonEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.BadmintonEvents
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Player1)
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Player2)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeTeam)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayTeam)
            .Include(e => e.Photos)
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .Include(e => e.FirstPlaceTeam)
            .Include(e => e.SecondPlaceTeam)
            .Include(e => e.ThirdPlaceTeam)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(BadmintonEvent), id);
        }

        return entity;
    }

    protected override async Task OnCreatedAsync(BadmintonEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з бадмінтону: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(BadmintonEvent oldEntity, BadmintonEvent newEntity, CancellationToken cancellationToken)
    {
        bool individualWinnersSet = newEntity.FirstPlaceId != null && newEntity.SecondPlaceId != null && newEntity.ThirdPlaceId != null;
        bool teamWinnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (individualWinnersSet || teamWinnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з бадмінтону: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }

    public async Task AddParticipantAsync(long eventId, BadmintonEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _dbcontext.BadmintonEventParticipants
            .AnyAsync(p => p.EventId == eventId && p.PlayerId == dto.PlayerId, cancellationToken);

        if (exists) return;

        var participant = dto.FromDto(eventId);
        _dbcontext.BadmintonEventParticipants.Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateParticipantAsync(long eventId, long playerId, BadmintonEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.BadmintonEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            participant.Points = dto.Points;
            participant.Rank = dto.Rank;
            _dbcontext.BadmintonEventParticipants.Update(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.BadmintonEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            _dbcontext.BadmintonEventParticipants.Remove(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }
}
