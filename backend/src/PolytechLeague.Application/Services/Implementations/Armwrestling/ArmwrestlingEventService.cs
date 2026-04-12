using PolytechLeague.Domain.Entities.Armwrestling;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Exceptions;

namespace PolytechLeague.Application.Services.Implementations.Armwrestling;

public class ArmwrestlingEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ArmwrestlingEvent>> _logger
) :
    AbstractEntityService<ArmwrestlingEvent>(_dbcontext, _logger),
    IArmwrestlingEventService
{
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
        
        oldEntity.FirstPlaceId = newEntity.FirstPlaceId;
        oldEntity.SecondPlaceId = newEntity.SecondPlaceId;
        oldEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
    }
}
