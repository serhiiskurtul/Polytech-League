using PolytechLeague.Domain.Entities.Football;
using PolytechLeague.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Football;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Params;

namespace PolytechLeague.Application.Services.Implementations.Football;

public class FootballMatchService :
    AbstractEntityService<FootballMatch>,
    IFootballMatchService
{
    public FootballMatchService(
        ApplicationDbContext dbcontext,
        ILogger<AbstractEntityService<FootballMatch>> logger
    ) : base(dbcontext, logger)
    {
    }
    public override async Task<IEnumerable<FootballMatch>> GetAllAsync(FilterParams? filterParams = null, SortParams? sortParams = null, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.FootballMatches
            .Include(m => m.Goals!)
                .ThenInclude(g => g.Player)
            .Include(m => m.Cards!)
                .ThenInclude(c => c.Player)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public override async Task<FootballMatch> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.FootballMatches
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .Include(m => m.HomeGoalkeeper)
            .Include(m => m.AwayGoalkeeper)
            .Include(m => m.HomeCaptain)
            .Include(m => m.AwayCaptain)
            .Include(m => m.Goals!)
                .ThenInclude(g => g.Player)
            .Include(m => m.Cards!)
                .ThenInclude(c => c.Player)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(nameof(FootballMatch), id);
        }

        return entity;
    }

    public override async Task<FootballMatch> CreateAsync(FootballMatch entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating Football match with rosters.");
        await MapRoster(entity, cancellationToken);

        // Fill default captain and goalkeeper from teams if not provided
        var homeTeam = await _dbcontext.FootballTeams.FindAsync(new object[] { entity.HomeTeamId }, cancellationToken);
        var awayTeam = await _dbcontext.FootballTeams.FindAsync(new object[] { entity.AwayTeamId }, cancellationToken);

        if (homeTeam != null)
        {
            entity.HomeCaptainId ??= homeTeam.CaptainId;
            entity.HomeGoalkeeperId ??= homeTeam.GoalkeeperId;
        }

        if (awayTeam != null)
        {
            entity.AwayCaptainId ??= awayTeam.CaptainId;
            entity.AwayGoalkeeperId ??= awayTeam.GoalkeeperId;
        }

        _dbcontext.FootballMatches.Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public override async Task UpdateAsync(long id, FootballMatch newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating Football match with rosters. Id: {Id}", id);

        var oldEntity = await _dbcontext.FootballMatches
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .Include(m => m.Goals)
            .Include(m => m.Cards)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        
        if (oldEntity == null)
        {
            throw new EntityNotFoundException(nameof(FootballMatch), id);
        }

        UpdateValues(oldEntity, newEntity);
        await MapRoster(oldEntity, newEntity, cancellationToken);
        SyncGoals(oldEntity, newEntity);
        _logger.LogInformation("UpdateAsync: Match {Id}. NewEntity Cards Count: {Count}", 
            oldEntity.Id, newEntity.Cards?.Count ?? 0);
        SyncCards(oldEntity, newEntity);
        _logger.LogInformation("UpdateAsync: Match {Id}. Cards synced. Count in oldEntity: {Count}", oldEntity.Id, oldEntity.Cards?.Count ?? 0);

        await _dbcontext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("UpdateAsync: Match {Id}. SaveChangesAsync completed.", oldEntity.Id);
    }

    private void SyncGoals(FootballMatch target, FootballMatch source)
    {
        if (source.Goals == null) return;

        target.Goals ??= new List<FootballGoal>();

        // Remove goals not in source
        var goalsToRemove = target.Goals
            .Where(tg => !source.Goals.Any(sg => sg.PlayerId == tg.PlayerId))
            .ToList();

        foreach (var goal in goalsToRemove)
        {
            _dbcontext.FootballGoals.Remove(goal);
        }

        // Add or update goals
        foreach (var sourceGoal in source.Goals)
        {
            var existingGoal = target.Goals.FirstOrDefault(tg => tg.PlayerId == sourceGoal.PlayerId);
            if (existingGoal != null)
            {
                existingGoal.GoalsCount = sourceGoal.GoalsCount;
            }
            else
            {
                sourceGoal.MatchId = target.Id;
                target.Goals.Add(sourceGoal);
            }
        }
    }

    private void SyncCards(FootballMatch target, FootballMatch source)
    {
        if (source.Cards == null)
        {
            _logger.LogWarning("SyncCards: Source cards collection is null for match {Id}", target.Id);
            return;
        }

        target.Cards ??= new List<FootballCard>();

        // Explicitly load and remove all cards for this match from the DB context
        // This ensures no stale cards remain regardless of collection state
        var matchId = target.Id;
        var existingCards = _dbcontext.FootballCards.Where(c => c.MatchId == matchId).ToList();
        
        _logger.LogInformation("SyncCards: Removing {Count} existing cards for match {Id}", existingCards.Count, matchId);
        if (existingCards.Any())
        {
            _dbcontext.FootballCards.RemoveRange(existingCards);
        }
        target.Cards.Clear();

        // Add new cards
        foreach (var sourceCard in source.Cards)
        {
            _logger.LogInformation("SyncCards: Adding card for player {PlayerId}, Type {Type} to match {MatchId}", 
                sourceCard.PlayerId, sourceCard.Type, matchId);
            
            target.Cards.Add(new FootballCard
            {
                MatchId = matchId,
                PlayerId = sourceCard.PlayerId,
                Type = sourceCard.Type
            });
        }
    }

    private async Task MapRoster(FootballMatch target, FootballMatch source, CancellationToken cancellationToken)
    {
        if (source.HomeRosterIds != null)
        {
            target.HomeRoster = await _dbcontext.Players
                .Where(p => source.HomeRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        if (source.AwayRosterIds != null)
        {
            target.AwayRoster = await _dbcontext.Players
                .Where(p => source.AwayRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }
    }

    private async Task MapRoster(FootballMatch entity, CancellationToken cancellationToken)
    {
        if (entity.HomeRosterIds != null)
        {
            entity.HomeRoster = await _dbcontext.Players
                .Where(p => entity.HomeRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        if (entity.AwayRosterIds != null)
        {
            entity.AwayRoster = await _dbcontext.Players
                .Where(p => entity.AwayRosterIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }
    }

    protected override void UpdateValues(FootballMatch oldEntity, FootballMatch newEntity)
    {
        oldEntity.EventId = newEntity.EventId;
        oldEntity.HomeTeamId = newEntity.HomeTeamId;
        oldEntity.AwayTeamId = newEntity.AwayTeamId;
        oldEntity.HomeTeamScore = newEntity.HomeTeamScore;
        oldEntity.AwayTeamScore = newEntity.AwayTeamScore;
        oldEntity.WinnerTeamId = newEntity.WinnerTeamId;
        oldEntity.Stage = newEntity.Stage;
        oldEntity.HomeGoalkeeperId = newEntity.HomeGoalkeeperId;
        oldEntity.AwayGoalkeeperId = newEntity.AwayGoalkeeperId;
        oldEntity.HomeCaptainId = newEntity.HomeCaptainId;
        oldEntity.AwayCaptainId = newEntity.AwayCaptainId;
    }
}

