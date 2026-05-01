using PolytechLeague.Domain.Entities.Futsal;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;

namespace PolytechLeague.Application.Services.Implementations.Futsal;

public class FutsalEventService :
    AbstractEntityService<FutsalEvent>,
    IFutsalEventService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ILogger<AbstractEntityService<FutsalEvent>> _logger;
    private readonly INotificationService _notificationService;

    public FutsalEventService(
        ApplicationDbContext dbcontext,
        ILogger<AbstractEntityService<FutsalEvent>> logger,
        INotificationService notificationService
    ) : base(dbcontext, logger, notificationService)
    {
        _dbcontext = dbcontext;
        _logger = logger;
        _notificationService = notificationService;
    }
    protected override void UpdateValues(FutsalEvent oldEntity, FutsalEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.RegistrationStart = newEntity.RegistrationStart;
        oldEntity.RegistrationEnd = newEntity.RegistrationEnd;
        oldEntity.EndDate = newEntity.EndDate;
        oldEntity.Level = newEntity.Level;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.Schedule = newEntity.Schedule;
        oldEntity.FirstPlaceTeamId = newEntity.FirstPlaceTeamId;
        oldEntity.SecondPlaceTeamId = newEntity.SecondPlaceTeamId;
        oldEntity.ThirdPlaceTeamId = newEntity.ThirdPlaceTeamId;
        oldEntity.IsModerated = newEntity.IsModerated;

        oldEntity.IsPublic = newEntity.IsPublic;
        oldEntity.IsTeam = newEntity.IsTeam;
        oldEntity.SeasonId = newEntity.SeasonId;
        oldEntity.Category = newEntity.Category;
    }

    public override async Task<FutsalEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.FutsalEvents
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Players)
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Captain)
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Goalkeeper)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeTeam)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayTeam)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeRoster)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayRoster)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeGoalkeeper)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayGoalkeeper)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeCaptain)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayCaptain)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Goals!)
                    .ThenInclude(g => g.Player)
            .Include(e => e.Photos)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(FutsalEvent), id);
        }

        return entity;
    }

    protected override async Task OnCreatedAsync(FutsalEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з футзалу: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(FutsalEvent oldEntity, FutsalEvent newEntity, CancellationToken cancellationToken)
    {
        // Update ranks for teams based on winners
        var eventWithTeams = await _dbcontext.FutsalEvents
            .Include(e => e.Teams)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);

        if (eventWithTeams != null && eventWithTeams.Teams != null)
        {
            foreach (var team in eventWithTeams.Teams)
            {
                if (team.Id == newEntity.FirstPlaceTeamId) team.Rank = 1;
                else if (team.Id == newEntity.SecondPlaceTeamId) team.Rank = 2;
                else if (team.Id == newEntity.ThirdPlaceTeamId) team.Rank = 3;
                else if (newEntity.FirstPlaceTeamId != null) team.Rank = 4;
                else team.Rank = 0;
            }
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        bool winnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (winnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з футзалу: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.FutsalEvents
            .Include(e => e.Teams!)
            .ThenInclude(t => t.Players)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (tournament == null) throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(FutsalEvent), eventId);

        if (tournament.Level != CompetitionLevel.CollegeGroup)
        {
            throw new InvalidOperationException("Self-registration is only allowed for college group tournaments.");
        }

        var player = await _dbcontext.Players
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId, cancellationToken)
            ?? throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(Player), playerId);

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

        var groupName = player.Group;
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new InvalidOperationException("Player must have a group to register for inter-group competitions.");
        }

        tournament.Teams ??= new List<FutsalTeam>();

        // Check if player is already in any team for this event
        var alreadyRegistered = tournament.Teams.Any(t => t.Players?.Any(p => p.Id == playerId) == true);
        if (alreadyRegistered) return;

        var team = tournament.Teams.FirstOrDefault(t => t.Name == groupName);
        if (team == null)
        {
            team = new FutsalTeam
            {
                Name = groupName,
                EventId = eventId,
                Players = new List<Player>()
            };
            tournament.Teams.Add(team);
        }

        team.Players ??= new List<Player>();
        team.Players.Add(player);

        await _dbcontext.SaveChangesAsync(cancellationToken);
        
        await _notificationService.SendNotificationToAdminsAsync(
            "Нова реєстрація на футзал",
            $"Гравець {player.FirstName} {player.LastName} (група {groupName}) зареєструвався на {tournament.Name}",
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }

    public override async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        // 0. Clear player references using raw SQL (as ExecuteUpdate doesn't support TPT hierarchies like Player)
        await _dbcontext.Database.ExecuteSqlRawAsync(
            $"UPDATE players SET futsal_team_id = NULL WHERE futsal_team_id IN (SELECT id FROM futsal_teams WHERE \"EventId\" = {id})", 
            cancellationToken);

        // 1. Fetch event with matches to handle Restrict constraints
        var entity = await _dbcontext.FutsalEvents
            .Include(e => e.Matches)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null) throw new EntityNotFoundException(nameof(FutsalEvent), id);

        // 2. Clear winners (Restrict constraint)
        entity.FirstPlaceTeamId = null;
        entity.SecondPlaceTeamId = null;
        entity.ThirdPlaceTeamId = null;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        // 3. Delete matches explicitly (Restrict constraint on Teams)
        if (entity.Matches != null && entity.Matches.Any())
        {
            _dbcontext.FutsalMatches.RemoveRange(entity.Matches);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        // 4. Finally delete the event
        _dbcontext.FutsalEvents.Remove(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }
}
