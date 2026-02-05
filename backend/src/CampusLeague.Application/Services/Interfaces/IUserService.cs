using CampusLeague.Domain.Entities;
using CampusLeague.Domain.Params;

namespace CampusLeague.Application.Services.Interfaces;

public interface IUserService : IAbstractEntityService<User>
{
    Task<IEnumerable<User>> GetAllWithDeletedAsync(
        SortParams? sortParams = null, CancellationToken cancellationToken = default);

    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task SoftDeleteAsync(long id, CancellationToken cancellationToken = default);

    Task ConfirmEmailAsync(string email, CancellationToken cancellationToken = default);
}

