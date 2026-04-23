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


namespace PolytechLeague.Application.Services.Implementations.Armwrestling;

public class ArmwrestlingEventService :
    AbstractEntityService<ArmwrestlingEvent>,
    IArmwrestlingEventService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ILogger<AbstractEntityService<ArmwrestlingEvent>> _logger;
    private readonly INotificationService _notificationService;

    public ArmwrestlingEventService(
        ApplicationDbContext dbcontext,
        ILogger<AbstractEntityService<ArmwrestlingEvent>> logger,
        INotificationService notificationService
    ) : base(dbcontext, logger, notificationService)
    {
        _dbcontext = dbcontext;
        _logger = logger;
        _notificationService = notificationService;
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
            .Include(e => e.ThirdPlace)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            _logger.LogWarning("ArmwrestlingEvent with id {Id} not found", id);
            throw new EntityNotFoundException(nameof(ArmwrestlingEvent), id);
        }

        return entity;
    }

    protected override void UpdateValues(ArmwrestlingEvent oldEntity, ArmwrestlingEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.RegistrationStart = newEntity.RegistrationStart;
        oldEntity.RegistrationEnd = newEntity.RegistrationEnd;
        oldEntity.EndDate = newEntity.EndDate;
        oldEntity.Level = newEntity.Level;
        oldEntity.Hand = newEntity.Hand;
        oldEntity.WeightClass = newEntity.WeightClass;
        oldEntity.Category = newEntity.Category;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.Schedule = newEntity.Schedule;
        oldEntity.TimeControl = newEntity.TimeControl;
        oldEntity.ParticipantsCount = newEntity.ParticipantsCount;
        oldEntity.IsModerated = newEntity.IsModerated;
        oldEntity.IsPublic = newEntity.IsPublic;
        
        oldEntity.FirstPlaceId = newEntity.FirstPlaceId;
        oldEntity.SecondPlaceId = newEntity.SecondPlaceId;
        oldEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
    }

    protected override async Task OnCreatedAsync(ArmwrestlingEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з армреслінгу: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(ArmwrestlingEvent oldEntity, ArmwrestlingEvent newEntity, CancellationToken cancellationToken)
    {
        // Перевірка чи були визначені призери (перехід від null до значення)
        bool winnersSet = newEntity.FirstPlaceId != null && newEntity.SecondPlaceId != null && newEntity.ThirdPlaceId != null;
        
        if (winnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з армреслінгу: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
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

        var exists = await _dbcontext.ArmwrestlingEventParticipants
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

        if (exists) return;

        var status = (tournament.IsModerated && !tournament.IsPublic) ? RegistrationStatus.Pending : RegistrationStatus.Approved;

        var participant = new ArmwrestlingEventParticipant
        {
            TournamentId = eventId,
            PlayerId = playerId,
            Status = status,
            GroupName = player.Group
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
                    $"Вашу заявку на участь у змаганні з армреслінгу схвалено!",
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
                    $"Вашу заявку на участь у змаганні з армреслінгу було відхилено.",
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
            ? "Вашу заявку на участь у змаганні з армреслінгу схвалено!"
            : "Вашу заявку на участь у змаганні з армреслінгу було відхилено.";

        await _notificationService.SendNotificationToUserAsync(
            player.UserId.Value,
            title,
            message,
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }
}
