using PolytechLeague.Domain.Entities.Futsal;
using PolytechLeague.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.Futsal;

public class FutsalMatchService :
    AbstractEntityService<FutsalMatch>,
    IFutsalMatchService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ILogger<AbstractEntityService<FutsalMatch>> _logger;

    public FutsalMatchService(
        ApplicationDbContext dbcontext,
        ILogger<AbstractEntityService<FutsalMatch>> logger
    ) : base(dbcontext, logger)
    {
        _dbcontext = dbcontext;
        _logger = logger;
    }
    public override async Task<FutsalMatch> CreateAsync(FutsalMatch entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating futsal match with rosters.");
        await MapRoster(entity, cancellationToken);

        // Fill default captain and goalkeeper from teams if not provided
        var homeTeam = await _dbcontext.FutsalTeams.FindAsync(new object[] { entity.HomeTeamId }, cancellationToken);
        var awayTeam = await _dbcontext.FutsalTeams.FindAsync(new object[] { entity.AwayTeamId }, cancellationToken);

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

        _dbcontext.FutsalMatches.Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public override async Task UpdateAsync(long id, FutsalMatch newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating futsal match with rosters. Id: {Id}", id);

        var oldEntity = await _dbcontext.FutsalMatches
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .Include(m => m.Goals)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        
        if (oldEntity == null)
        {
            throw new EntityNotFoundException(nameof(FutsalMatch), id);
        }

        UpdateValues(oldEntity, newEntity);
        await MapRoster(oldEntity, newEntity, cancellationToken);
        SyncGoals(oldEntity, newEntity);

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    private void SyncGoals(FutsalMatch target, FutsalMatch source)
    {
        if (source.Goals == null) return;

        target.Goals ??= new List<FutsalGoal>();

        // Remove goals not in source
        var goalsToRemove = target.Goals
            .Where(tg => !source.Goals.Any(sg => sg.PlayerId == tg.PlayerId))
            .ToList();

        foreach (var goal in goalsToRemove)
        {
            _dbcontext.FutsalGoals.Remove(goal);
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

    private async Task MapRoster(FutsalMatch target, FutsalMatch source, CancellationToken cancellationToken)
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

    private async Task MapRoster(FutsalMatch entity, CancellationToken cancellationToken)
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

    protected override void UpdateValues(FutsalMatch oldEntity, FutsalMatch newEntity)
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

