using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto.AthleticsDto;
using PolytechLeague.Application.Services.Interfaces.Athletics;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities.Athletics;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Params;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Domain.Entities.Enums;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.Athletics;

public class AthleticsEventService(
    ApplicationDbContext context,
    ILogger<AbstractEntityService<AthleticsEvent>> logger,
    INotificationService notificationService) 
    : AbstractEntityService<AthleticsEvent>(context, logger, notificationService), IAthleticsEventService
{
    public override async Task<AthleticsEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.AthleticsEvents
            .Include(e => e.Results).ThenInclude(r => r.Player)
            .Include(e => e.Participants).ThenInclude(p => p.Player)
            .Include(e => e.Photos)
            .Include(e => e.Season)
            .Include(e => e.Mvp)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)

            ?? throw new Exception("Athletics event not found");
    }

    public override async Task<PageResult<AthleticsEvent>> GetPageAsync(FilterParams? filterParams, SortParams? sortParams, PageParams? pageParams, CancellationToken cancellationToken = default)
    {
        var query = _dbcontext.AthleticsEvents
            .Include(e => e.Photos)
            .Include(e => e.Season)
            .AsQueryable();

        // Basic filtering could be added here if needed

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(e => e.Date)
            .Skip(((pageParams?.Page ?? 1) - 1) * (pageParams?.PageSize ?? 10))
            .Take(pageParams?.PageSize ?? 10)
            .ToListAsync(cancellationToken);

        return new PageResult<AthleticsEvent>(items, totalCount);
    }

    protected override void UpdateValues(AthleticsEvent existing, AthleticsEvent entity)
    {
        existing.Name = entity.Name;
        existing.Date = entity.Date;
        existing.Location = entity.Location;
        existing.Description = entity.Description;
        existing.RefereeIds = entity.RefereeIds;
        existing.Level = entity.Level;
        existing.Category = entity.Category;
        existing.RegistrationStart = entity.RegistrationStart;
        existing.RegistrationEnd = entity.RegistrationEnd;
        existing.EndDate = entity.EndDate;
        existing.Rules = entity.Rules;
        existing.Prizes = entity.Prizes;
        existing.HeadRefereeName = entity.HeadRefereeName;
        existing.Referees = entity.Referees;
        existing.Schedule = entity.Schedule;
        existing.IsTeam = entity.IsTeam;
        existing.IsModerated = entity.IsModerated;
        existing.IsPublic = entity.IsPublic;
        existing.SeasonId = entity.SeasonId;
        existing.MvpId = entity.MvpId;
    }


    protected override async Task OnCreatedAsync(AthleticsEvent entity, CancellationToken cancellationToken)
    {
        if (_notificationService != null)
        {
            if (!entity.DisableNotifications)
        {
            await _notificationService!.SendNotificationToAllAsync(
                "Нове змагання!",
                $"Створено нове змагання з легкої атлетики: {entity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{entity.Id}",
                true
            );
        }
        }
    }









    public async Task AddResultAsync(long eventId, AthleticsResultDto dto, CancellationToken cancellationToken = default)
    {
        var result = new AthleticsResult
        {
            AthleticsEventId = eventId,
            DisciplineName = dto.DisciplineName,
            PlayerId = dto.PlayerId,
            TeamName = dto.TeamName,
            Score = dto.Score,
            Rank = dto.Rank
        };

        await _dbcontext.AthleticsResults.AddAsync(result, cancellationToken);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateResultAsync(long eventId, long resultId, AthleticsResultDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _dbcontext.AthleticsResults.FindAsync(new object[] { resultId }, cancellationToken)
            ?? throw new Exception("Result not found");

        existing.DisciplineName = dto.DisciplineName;
        existing.PlayerId = dto.PlayerId;
        existing.TeamName = dto.TeamName;
        existing.Score = dto.Score;
        existing.Rank = dto.Rank;

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveResultAsync(long eventId, long resultId, CancellationToken cancellationToken = default)
    {
        var existing = await _dbcontext.AthleticsResults.FindAsync(new object[] { resultId }, cancellationToken)
            ?? throw new Exception("Result not found");

        _dbcontext.AthleticsResults.Remove(existing);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddParticipantAsync(long eventId, AthleticsEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var participant = new AthleticsEventParticipant
        {
            AthleticsEventId = eventId,
            PlayerId = dto.PlayerId,
            GroupName = dto.GroupName
        };

        await _dbcontext.AthleticsEventParticipants.AddAsync(participant, cancellationToken);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var existing = await _dbcontext.AthleticsEventParticipants.FindAsync(new object[] { eventId, playerId }, cancellationToken)
            ?? throw new Exception("Participant not found");

        _dbcontext.AthleticsEventParticipants.Remove(existing);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }
}
