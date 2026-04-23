using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities.Shared;
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
        oldEntity.IsModerated = newEntity.IsModerated;
        oldEntity.IsPublic = newEntity.IsPublic;
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
        if (string.IsNullOrEmpty(participant.GroupName))
        {
            var player = await _dbcontext.Set<Player>().FindAsync(dto.PlayerId);
            participant.GroupName = player?.Group;
        }
        _dbcontext.BadmintonEventParticipants.Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        if (dto.Status == RegistrationStatus.Approved || dto.Status == RegistrationStatus.Rejected)
        {
            await SendParticipantStatusNotificationAsync(eventId, dto.PlayerId, dto.Status, cancellationToken);
        }
    }

    public async Task UpdateParticipantAsync(long eventId, long playerId, BadmintonEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.BadmintonEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            var oldStatus = participant.Status;
            participant.Points = dto.Points;
            participant.Rank = dto.Rank;
            participant.Status = dto.Status;
            _dbcontext.BadmintonEventParticipants.Update(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);

            if (oldStatus != dto.Status && (dto.Status == RegistrationStatus.Approved || dto.Status == RegistrationStatus.Rejected))
            {
                await SendParticipantStatusNotificationAsync(eventId, playerId, dto.Status, cancellationToken);
            }
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

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.BadmintonEvents.FindAsync([eventId], cancellationToken)
            ?? throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(BadmintonEvent), eventId);

        if (tournament.Level != CompetitionLevel.CollegeGroup)
        {
            throw new InvalidOperationException("Self-registration is only allowed for college group tournaments.");
        }

        var exists = await _dbcontext.BadmintonEventParticipants
            .AnyAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);

        if (exists) return;

        var status = (tournament.IsModerated && !tournament.IsPublic) ? RegistrationStatus.Pending : RegistrationStatus.Approved;

        var participant = new BadmintonEventParticipant
        {
            EventId = eventId,
            PlayerId = playerId,
            Status = status
        };

        var player = await _dbcontext.Set<Player>().FindAsync([playerId], cancellationToken);
        participant.GroupName = player?.Group;

        _dbcontext.BadmintonEventParticipants.Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        if (tournament.IsModerated)
        {
            await _notificationService.SendNotificationToAdminsAsync(
                "Новий запит на реєстрацію",
                $"Гравець {player?.FirstName} {player?.LastName} хоче зареєструватися на {tournament.Name}",
                NotificationType.Tournament,
                $"/tournaments/{eventId}"
            );
        }
    }

    public async Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.BadmintonEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);

        if (participant != null)
        {
            participant.Status = RegistrationStatus.Approved;
            await _dbcontext.SaveChangesAsync(cancellationToken);

            var player = await _dbcontext.Set<Player>().FindAsync([playerId], cancellationToken);
            if (player?.UserId != null)
            {
                await _notificationService.SendNotificationToUserAsync(
                    player.UserId.Value,
                    "Реєстрація схвалена",
                    $"Вашу заявку на участь у змаганні з бадмінтону схвалено!",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    public async Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.BadmintonEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);

        if (participant != null)
        {
            participant.Status = RegistrationStatus.Rejected;
            await _dbcontext.SaveChangesAsync(cancellationToken);

            var player = await _dbcontext.Set<Player>().FindAsync([playerId], cancellationToken);
            if (player?.UserId != null)
            {
                await _notificationService.SendNotificationToUserAsync(
                    player.UserId.Value,
                    "Реєстрація відхилена",
                    $"Вашу заявку на участь у змаганні з бадмінтону було відхилено.",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    private async Task SendParticipantStatusNotificationAsync(long eventId, long playerId, RegistrationStatus status, CancellationToken cancellationToken)
    {
        var player = await _dbcontext.Set<Player>().FindAsync([playerId], cancellationToken);
        if (player?.UserId == null) return;

        string title = status == RegistrationStatus.Approved ? "Реєстрація схвалена" : "Реєстрація відхилена";
        string message = status == RegistrationStatus.Approved
            ? "Вашу заявку на участь у змаганні з бадмінтону схвалено!"
            : "Вашу заявку на участь у змаганні з бадмінтону було відхилено.";

        await _notificationService.SendNotificationToUserAsync(
            player.UserId.Value,
            title,
            message,
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }
}
