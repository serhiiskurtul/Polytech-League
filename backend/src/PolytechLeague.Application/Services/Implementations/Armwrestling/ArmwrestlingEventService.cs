using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Shared;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Application.Dto.ArmwrestlingDto;
using PolytechLeague.Domain.Entities.Enums;


namespace PolytechLeague.Application.Services.Implementations.Armwrestling;

public class ArmwrestlingEventService :
    AbstractEntityService<ArmwrestlingEvent>,
    IArmwrestlingEventService
{


    public ArmwrestlingEventService(
        ApplicationDbContext dbcontext,
        ILogger<AbstractEntityService<ArmwrestlingEvent>> logger,
        INotificationService notificationService
    ) : base(dbcontext, logger, notificationService)
    {
    }

    public override async Task<ArmwrestlingEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching ArmwrestlingEvent with id {Id} including matches and players", id);

        var entity = await _dbcontext.ArmwrestlingEvents
            .Include(e => e.Photos)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player1)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player2)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .Include(e => e.Mvp)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);


        if (entity is null)
        {
            _logger.LogWarning("ArmwrestlingEvent with id {Id} not found", id);
            throw new EntityNotFoundException(nameof(ArmwrestlingEvent), id);
        }

        return entity;
    }

    protected override void UpdateValues(ArmwrestlingEvent updatedEntity, ArmwrestlingEvent newEntity)
    {
        updatedEntity.Name = newEntity.Name;
        updatedEntity.Date = newEntity.Date;
        updatedEntity.Location = newEntity.Location;
        updatedEntity.Description = newEntity.Description;
        updatedEntity.RegistrationStart = newEntity.RegistrationStart;
        updatedEntity.RegistrationEnd = newEntity.RegistrationEnd;
        updatedEntity.EndDate = newEntity.EndDate;
        updatedEntity.Level = newEntity.Level;
        updatedEntity.Hand = newEntity.Hand;
        updatedEntity.WeightClass = newEntity.WeightClass;
        updatedEntity.Category = newEntity.Category;
        updatedEntity.Rules = newEntity.Rules;
        updatedEntity.Prizes = newEntity.Prizes;
        updatedEntity.HeadRefereeName = newEntity.HeadRefereeName;
        updatedEntity.Referees = newEntity.Referees;
        updatedEntity.Schedule = newEntity.Schedule;
        updatedEntity.TimeControl = newEntity.TimeControl;
        updatedEntity.ParticipantsCount = newEntity.ParticipantsCount;
        updatedEntity.IsModerated = newEntity.IsModerated;
        updatedEntity.IsPublic = newEntity.IsPublic;
        updatedEntity.IsTeam = newEntity.IsTeam;
        
        updatedEntity.FirstPlaceId = newEntity.FirstPlaceId;

        updatedEntity.SecondPlaceId = newEntity.SecondPlaceId;
        updatedEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
        updatedEntity.SeasonId = newEntity.SeasonId;
        updatedEntity.MvpId = newEntity.MvpId;
    }


    protected override async Task OnCreatedAsync(ArmwrestlingEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з армрестлінгу: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}",
            true
        );
    }

    protected override async Task OnUpdatedAsync(ArmwrestlingEvent originalEntity, ArmwrestlingEvent updatedEntity, CancellationToken cancellationToken)
    {
        // Update ranks for participants based on winners
        var eventWithParticipants = await _dbcontext.ArmwrestlingEvents
            .Include(e => e.Players)
            .FirstOrDefaultAsync(e => e.Id == updatedEntity.Id, cancellationToken);

        if (eventWithParticipants != null && eventWithParticipants.Players != null)
        {
            foreach (var participant in eventWithParticipants.Players)
            {
                if (participant.PlayerId == updatedEntity.FirstPlaceId) participant.Rank = 1;
                else if (participant.PlayerId == updatedEntity.SecondPlaceId) participant.Rank = 2;
                else if (participant.PlayerId == updatedEntity.ThirdPlaceId) participant.Rank = 3;
                else participant.Rank = 0;
            }
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        // Перевірка чи були визначені призери (перехід від null до значення)
        bool winnersSet = updatedEntity.FirstPlaceId != null && updatedEntity.SecondPlaceId != null && updatedEntity.ThirdPlaceId != null && (originalEntity.FirstPlaceId == null || originalEntity.SecondPlaceId == null || originalEntity.ThirdPlaceId == null);
        
        if (winnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з армрестлінгу: {updatedEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{updatedEntity.Id}",
                true
            );
        }
    }

    public async Task AddParticipantAsync(long eventId, ArmwrestlingEventParticipantDto dto, CancellationToken cancellationToken)
    {
        var exists = await _dbcontext.ArmwrestlingEventParticipants
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == dto.PlayerId, cancellationToken);
        
        if (exists) return;
 
        var player = await _dbcontext.Set<Player>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == dto.PlayerId, cancellationToken);
            
        if (player?.User?.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Administrators cannot be participants in tournaments.");
        }

        var participant = new ArmwrestlingEventParticipant
        {
            TournamentId = eventId,
            PlayerId = dto.PlayerId,
            Weight = dto.Weight,
            Rank = dto.Rank,
            GroupName = dto.GroupName,
            Status = dto.Status
        };

        if (string.IsNullOrEmpty(participant.GroupName))
        {
            participant.GroupName = player?.Group;
        }
        
        participant.PlayerFirstName = dto.FirstName ?? player?.FirstName;
        participant.PlayerLastName = dto.LastName ?? player?.LastName;

        _dbcontext.ArmwrestlingEventParticipants.Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        if (dto.Status == RegistrationStatus.Approved || dto.Status == RegistrationStatus.Rejected)
        {
            await SendParticipantStatusNotificationAsync(eventId, dto.PlayerId, dto.Status, cancellationToken);
        }
    }

    public async Task UpdateParticipantAsync(long eventId, long playerId, ArmwrestlingEventParticipantDto dto, CancellationToken cancellationToken)
    {
        var participant = await _dbcontext.Set<ArmwrestlingEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            var oldStatus = participant.Status;
            participant.Weight = dto.Weight;
            participant.Rank = dto.Rank;
            participant.Status = dto.Status;
            participant.GroupName = dto.GroupName;

            if (!string.IsNullOrEmpty(dto.FirstName)) participant.PlayerFirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName)) participant.PlayerLastName = dto.LastName;

            _dbcontext.Set<ArmwrestlingEventParticipant>().Update(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);

            if (oldStatus != dto.Status && (dto.Status == RegistrationStatus.Approved || dto.Status == RegistrationStatus.Rejected))
            {
                await SendParticipantStatusNotificationAsync(eventId, playerId, dto.Status, cancellationToken);
            }
        }
    }

    public async Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken)
    {
        var participant = await _dbcontext.ArmwrestlingEventParticipants
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

        if (participant != null)
        {
            _dbcontext.ArmwrestlingEventParticipants.Remove(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.ArmwrestlingEvents.FindAsync([eventId], cancellationToken)
            ?? throw new EntityNotFoundException(nameof(ArmwrestlingEvent), eventId);

        if (tournament.Level != CompetitionLevel.CollegeGroup)
        {
            throw new InvalidOperationException("Self-registration is only allowed for college group tournaments.");
        }

        var player = await _dbcontext.Set<Player>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Player), playerId);

        if (player.User?.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Administrators cannot register for tournaments.");
        }

        if (player.Group == "Гість")
        {
            throw new InvalidOperationException("Користувачі з групою 'Гість' не можуть реєструватися на змагання. Будь ласка, оновіть свою групу в профілі.");
        }

        if (string.IsNullOrWhiteSpace(player.StudentId))
        {
            throw new InvalidOperationException("Для реєстрації на змагання необхідно вказати серію та номер студентського квитка в профілі (наприклад: PH12345678).");
        }

        if (player.Gender == Gender.None)
        {
            throw new InvalidOperationException("Для реєстрації на змагання необхідно вказати стать у профілі.");
        }

        var exists = await _dbcontext.ArmwrestlingEventParticipants
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

        if (exists) return;

        var status = (tournament.IsModerated && !tournament.IsPublic) ? RegistrationStatus.Pending : RegistrationStatus.Approved;

        var participant = new ArmwrestlingEventParticipant
        {
            TournamentId = eventId,
            PlayerId = playerId,
            Status = status,
            GroupName = player.Group,
            PlayerFirstName = player.FirstName,
            PlayerLastName = player.LastName
        };

        _dbcontext.ArmwrestlingEventParticipants.Add(participant);
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
        var participant = await _dbcontext.ArmwrestlingEventParticipants
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

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
                    $"Вашу заявку на участь у змаганні з армрестлінгу схвалено!",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    public async Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.ArmwrestlingEventParticipants
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

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
                    $"Вашу заявку на участь у змаганні з армрестлінгу було відхилено.",
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
            ? "Вашу заявку на участь у змаганні з армрестлінгу схвалено!"
            : "Вашу заявку на участь у змаганні з армрестлінгу було відхилено.";

        await _notificationService.SendNotificationToUserAsync(
            player.UserId.Value,
            title,
            message,
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }
}
