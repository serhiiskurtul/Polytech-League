using PolytechLeague.Domain.Entities.Checkers;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.Checkers;

public class CheckersPlayerService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<CheckersPlayer>> _logger
) :
    AbstractEntityService<CheckersPlayer>(_dbcontext, _logger),
    ICheckersPlayerService
{
    protected override void UpdateValues(CheckersPlayer oldEntity, CheckersPlayer newEntity)
    {
        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
        oldEntity.BirthDate = newEntity.BirthDate;
        oldEntity.PhotoUrl = newEntity.PhotoUrl;
        oldEntity.City = newEntity.City;
        oldEntity.Group = newEntity.Group;
        oldEntity.Rating = newEntity.Rating;
        oldEntity.Title = newEntity.Title;
    }
}
