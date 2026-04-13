using PolytechLeague.Domain.Entities.Badminton;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Services.Implementations.Badminton;

public class BadmintonEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BadmintonEvent>> _logger
) :
    AbstractEntityService<BadmintonEvent>(_dbcontext, _logger),
    IBadmintonEventService
{
    protected override void UpdateValues(BadmintonEvent oldEntity, BadmintonEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TournamentType = newEntity.TournamentType;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.Schedule = newEntity.Schedule;
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
                .ThenInclude(m => m.Team1)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Team2)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeRoster)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayRoster)
            .Include(e => e.Photos)
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(BadmintonEvent), id);
        }

        return entity;
    }
}
