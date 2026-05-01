using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Shared;
using PolytechLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Chess;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Utilities;

namespace PolytechLeague.Application.Services.Implementations.Chess;

public class ChessEventService :
    AbstractEntityService<ChessEvent>,
    IChessEventService
{
    private readonly IExcelImportService _excelImportService;
 
     public ChessEventService(
         ApplicationDbContext dbcontext,
         ILogger<AbstractEntityService<ChessEvent>> logger,
         INotificationService notificationService,
         IExcelImportService excelImportService
     ) : base(dbcontext, logger, notificationService)
     {
         _excelImportService = excelImportService;
     }
    protected override void UpdateValues(ChessEvent oldEntity, ChessEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.RegistrationStart = newEntity.RegistrationStart;
        oldEntity.RegistrationEnd = newEntity.RegistrationEnd;
        oldEntity.EndDate = newEntity.EndDate;
        oldEntity.Level = newEntity.Level;
        oldEntity.TimeControl = newEntity.TimeControl;
        oldEntity.System = newEntity.System;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.RefereeIds = newEntity.RefereeIds;
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
        oldEntity.SeasonId = newEntity.SeasonId;
        oldEntity.Category = newEntity.Category;
    }

    public override async Task<ChessEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.Set<ChessEvent>()
            .Include(e => e.Photos)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Players)
            .Include(e => e.Games!)
                .ThenInclude(g => g.WhitePlayer)
            .Include(e => e.Games!)
                .ThenInclude(g => g.BlackPlayer)
            .Include(e => e.Games!)
                .ThenInclude(g => g.WhiteTeam)
            .Include(e => e.Games!)
                .ThenInclude(g => g.BlackTeam)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .Include(e => e.FirstPlaceTeam)
            .Include(e => e.SecondPlaceTeam)
            .Include(e => e.ThirdPlaceTeam)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(ChessEvent), id);
    }

    public async Task AddParticipantAsync(long eventId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _dbcontext.Set<ChessEventParticipant>()
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == dto.PlayerId, cancellationToken);

        if (exists) return;

        var player = await _dbcontext.Set<Player>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == dto.PlayerId, cancellationToken);
            
        if (player?.User?.Role == UserRole.Admin)
        {
            _logger.LogWarning("Skipping player {PlayerId} because they are an administrator.", dto.PlayerId);
            return;
        }

        var participant = new ChessEventParticipant
        {
            TournamentId = eventId,
            PlayerId = dto.PlayerId,
            Points = dto.Points,
            Rank = dto.Rank,
            GroupName = dto.GroupName,
            Status = dto.Status
        };

        if (string.IsNullOrEmpty(participant.GroupName))
        {
            participant.GroupName = player?.Group;
        }

        _dbcontext.Set<ChessEventParticipant>().Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        if (dto.Status == RegistrationStatus.Approved || dto.Status == RegistrationStatus.Rejected)
        {
            await SendParticipantStatusNotificationAsync(eventId, dto.PlayerId, dto.Status, cancellationToken);
        }
    }

    public async Task UpdateParticipantAsync(long eventId, long playerId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<ChessEventParticipant>()
            .Include(p => p.Player)
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
            
        if (participant != null)
        {
            var oldStatus = participant.Status;
            participant.Points = dto.Points;
            participant.Rank = dto.Rank;
            participant.Status = dto.Status;
            participant.GroupName = dto.GroupName;

            if (participant.Player != null)
            {
                if (!string.IsNullOrEmpty(dto.FirstName)) participant.Player.FirstName = dto.FirstName;
                if (!string.IsNullOrEmpty(dto.LastName)) participant.Player.LastName = dto.LastName;
                if (!string.IsNullOrEmpty(dto.GroupName)) participant.Player.Group = dto.GroupName;
                
                _dbcontext.Set<Player>().Update(participant.Player);
            }
            
            _dbcontext.Set<ChessEventParticipant>().Update(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);

            if (oldStatus != dto.Status && (dto.Status == RegistrationStatus.Approved || dto.Status == RegistrationStatus.Rejected))
            {
                await SendParticipantStatusNotificationAsync(eventId, playerId, dto.Status, cancellationToken);
            }
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

    protected override async Task OnCreatedAsync(ChessEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з шахів: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(ChessEvent oldEntity, ChessEvent newEntity, CancellationToken cancellationToken)
    {
        // Update ranks for participants based on winners
        var eventWithParticipants = await _dbcontext.Set<ChessEvent>()
            .Include(e => e.Players)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);

        if (eventWithParticipants != null && eventWithParticipants.Players != null)
        {
            foreach (var participant in eventWithParticipants.Players)
            {
                if (participant.PlayerId == newEntity.FirstPlaceId) participant.Rank = 1;
                else if (participant.PlayerId == newEntity.SecondPlaceId) participant.Rank = 2;
                else if (participant.PlayerId == newEntity.ThirdPlaceId) participant.Rank = 3;
                else participant.Rank = 0;
            }
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        bool individualWinnersSet = newEntity.FirstPlaceId != null && newEntity.SecondPlaceId != null && newEntity.ThirdPlaceId != null;
        bool teamWinnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (individualWinnersSet || teamWinnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з шахів: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }

    public async Task ImportDataFromUrlAsync(long eventId, string url, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Excel data import for event {EventId} from URL: {Url}", eventId, url);
        var data = await _excelImportService.ImportFromChessResultsUrlAsync(url, cancellationToken);
        await ProcessImportData(eventId, data, cancellationToken);
    }

    public async Task ImportDataFromFileAsync(long eventId, Stream stream, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Excel data import for event {EventId} from uploaded file", eventId);
        var data = await _excelImportService.ImportFromStreamAsync(stream, cancellationToken);
        await ProcessImportData(eventId, data, cancellationToken);
    }

    private async Task ProcessImportData(long eventId, ScrapedTournamentExcelData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing {PlayerCount} players for event {EventId}", 
            data.Players.Count, eventId);

        if (data.Players.Count == 0)
        {
            throw new InvalidOperationException("Не вдалося знайти список учасників або дані в Excel. Перевірте, чи файл містить заголовки 'Name', 'Ім'я', 'Прізвище' тощо.");
        }

        foreach (var sp in data.Players)
        {
            Player? player = null;
            string[] names;

            // 1. Try match by FIDE ID
            if (!string.IsNullOrEmpty(sp.FideId))
            {
                player = await _dbcontext.Set<Player>()
                    .FirstOrDefaultAsync(p => p.ChessFideId == sp.FideId, cancellationToken);
            }

            // 2. Try match by Name and Specialty
            if (player == null)
            {
                names = sp.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (names.Length >= 2)
                {
                    var lastName = names[0];
                    var firstName = names[1];
                    
                    var query = _dbcontext.Set<Player>()
                        .Where(p => p.LastName.ToLower() == lastName.ToLower() && p.FirstName.ToLower() == firstName.ToLower());

                    if (!string.IsNullOrEmpty(sp.Group))
                    {
                        var parsed = GroupUtility.ParseGroup(sp.Group, DateTime.Now);
                        if (parsed.specialty > 0)
                        {
                            player = await query.FirstOrDefaultAsync(p => p.SpecialtyCode == parsed.specialty && p.EnrollmentYear == parsed.enrollmentYear, cancellationToken);
                        }
                    }

                    if (player == null)
                    {
                        player = await query.FirstOrDefaultAsync(cancellationToken);
                    }
                }
            }

            // 3. Create if not found
            if (player == null)
            {
                names = sp.FullName.Replace(",", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var lastName = names.Length > 0 ? names[0] : "Невідомий";
                var firstName = names.Length > 1 ? names[1] : "Гравець";

                var parsed = !string.IsNullOrEmpty(sp.Group) ? GroupUtility.ParseGroup(sp.Group, DateTime.Now) : (specialty: 0, enrollmentYear: 0, groupNumber: 0);
                
                player = new Player
                {
                    LastName = lastName,
                    FirstName = firstName,
                    City = "Чернівці",
                    Group = sp.Group ?? "Unknown",
                    EnrollmentYear = parsed.enrollmentYear,
                    SpecialtyCode = parsed.specialty,
                    GroupNumber = parsed.groupNumber,
                    PhotoUrl = "/images/placeholders/user.png",
                    ChessFideId = sp.FideId,
                    ChessRating = sp.Rating,
                    ChessTitle = sp.Title
                };
                _dbcontext.Set<Player>().Add(player);
                await _dbcontext.SaveChangesAsync(cancellationToken);
            }

            // Add as participant if not already
            await AddParticipantAsync(eventId, new PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto
            {
                PlayerId = player.Id,
                Points = sp.Points,
                Rank = sp.Rank ?? 0,
                GroupName = sp.Group ?? player.Group,
                Status = RegistrationStatus.Approved
            }, cancellationToken);
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.Set<ChessEvent>().FindAsync([eventId], cancellationToken)
            ?? throw new EntityNotFoundException(nameof(ChessEvent), eventId);

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
            throw new InvalidOperationException("Для реєстрації на змагання необхідно вказати серію та номер студентського квитка в профілі (наприклад: PH13856483).");
        }

        var exists = await _dbcontext.Set<ChessEventParticipant>()
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

        if (exists) return;

        var status = (tournament.IsModerated && !tournament.IsPublic) ? RegistrationStatus.Pending : RegistrationStatus.Approved;

        var participant = new ChessEventParticipant
        {
            TournamentId = eventId,
            PlayerId = playerId,
            Status = status,
            GroupName = player.Group
        };

        _dbcontext.Set<ChessEventParticipant>().Add(participant);
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
        var participant = await _dbcontext.Set<ChessEventParticipant>()
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
                    $"Вашу заявку на участь у змаганні з шахів схвалено!",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    public async Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<ChessEventParticipant>()
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
                    $"Вашу заявку на участь у змаганні з шахів було відхилено.",
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
            ? "Вашу заявку на участь у змаганні з шахів схвалено!"
            : "Вашу заявку на участь у змаганні з шахів було відхилено.";

        await _notificationService.SendNotificationToUserAsync(
            player.UserId.Value,
            title,
            message,
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }
}
