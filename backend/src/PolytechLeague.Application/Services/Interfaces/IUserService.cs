using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Params;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IUserService : IAbstractEntityService<User>
{
    Task<IEnumerable<User>> GetAllWithDeletedAsync(
        SortParams? sortParams = null, CancellationToken cancellationToken = default);

    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task SoftDeleteAsync(long id, CancellationToken cancellationToken = default);
    Task BlockAsync(long id, CancellationToken cancellationToken = default);
    Task UnblockAsync(long id, CancellationToken cancellationToken = default);
    Task ConfirmEmailAsync(string email, CancellationToken cancellationToken = default);
    Task ChangeRoleAsync(long id, UserRole role, CancellationToken cancellationToken = default);
    Task UpdatePasswordAsync(long id, string newPassword, CancellationToken cancellationToken = default);
}
