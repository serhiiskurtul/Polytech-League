using PolytechLeague.Domain.Params;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IAbstractEntityService<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAsync(
        FilterParams? filterParams = null,
        SortParams? sortParams = null,
        CancellationToken cancellationToken = default);

    Task<PageResult<TEntity>> GetPageAsync(
        FilterParams? filterParams = null,
        SortParams? sortParams = null,
        PageParams? pageParams = null,
        CancellationToken cancellationToken = default);

    Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(long id, TEntity newEntity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
