using PolytechLeague.Application.Extensions;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Params;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations;

public abstract class AbstractEntityService<TEntity>(
    ApplicationDbContext dbcontext,
    ILogger<AbstractEntityService<TEntity>> logger,
    INotificationService? notificationService = null) 
    : IAbstractEntityService<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _dbcontext = dbcontext;
    protected readonly ILogger<AbstractEntityService<TEntity>> _logger = logger;
    protected readonly INotificationService? _notificationService = notificationService;


    protected virtual Task OnCreatedAsync(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task OnUpdatedAsync(TEntity originalEntity, TEntity updatedEntity, CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        FilterParams? filterParams = null,
        SortParams? sortParams = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching all entities of type {EntityType}", typeof(TEntity).Name);

        var entities = await _dbcontext.Set<TEntity>()
            .AsNoTracking()
            .Filter(filterParams)
            .Sort(sortParams)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Fetched {Count} entities of type {EntityType}", entities.Count, typeof(TEntity).Name);

        return entities;
    }

    public virtual async Task<PageResult<TEntity>> GetPageAsync(
        FilterParams? filterParams = null,
        SortParams? sortParams = null,
        PageParams? pageParams = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching paged entities of type {EntityType}", typeof(TEntity).Name);

        var entities = await _dbcontext.Set<TEntity>()
            .AsNoTracking()
            .Filter(filterParams)
            .Sort(sortParams)
            .Page(pageParams, cancellationToken);

        _logger.LogInformation("Fetched {Count} paged entities of type {EntityType}", entities.Data.Count, typeof(TEntity).Name);

        return entities;
    }

    public virtual async Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching entity of type {EntityType} with id {Id}", typeof(TEntity).Name, id);

        var entity = await _dbcontext.Set<TEntity>().FindAsync([id], cancellationToken);
        if (entity is null)
        {
            _logger.LogWarning("Entity of type {EntityType} with id {Id} not found", typeof(TEntity).Name, id);
            throw new EntityNotFoundException(typeof(TEntity).Name, id);
        }

        _logger.LogInformation("Entity of type {EntityType} with id {Id} successfully fetched", typeof(TEntity).Name, id);
        return entity;
    }

    public virtual async Task<TEntity> CreateAsync(
        TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new entity of type {EntityType}", typeof(TEntity).Name);

        _dbcontext.Set<TEntity>().Add(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        // Ensure the ID is populated (sometimes EF Core doesn't update it immediately in memory for TPT)
        await _dbcontext.Entry(entity).ReloadAsync(cancellationToken);

        // Fallback if ID is still 0 and it's an Event
        if (entity is Event ev && ev.Id == 0)
        {
            var id = await _dbcontext.Set<Event>()
                .Where(e => e.Name == ev.Name && e.Date == ev.Date)
                .OrderByDescending(e => e.Id)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (id != 0)
            {
                ev.Id = id;
            }
        }

        await OnCreatedAsync(entity, cancellationToken);

        _logger.LogInformation("Entity of type {EntityType} successfully created", typeof(TEntity).Name);
        return entity;
    }

    public virtual async Task UpdateAsync(
        long id, TEntity newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating entity of type {EntityType} with id {Id}", typeof(TEntity).Name, id);

        var originalEntity = await _dbcontext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(e => EF.Property<long>(e, "Id") == id, cancellationToken);
        
        var oldEntity = await _dbcontext.Set<TEntity>().FindAsync([id], cancellationToken);
        if (oldEntity is null)
        {
            _logger.LogWarning("Entity of type {EntityType} with id {Id} not found for update", typeof(TEntity).Name, id);
            throw new EntityNotFoundException(nameof(TEntity), id);
        }

        UpdateValues(oldEntity, newEntity);

        await _dbcontext.SaveChangesAsync(cancellationToken);

        await OnUpdatedAsync(originalEntity, oldEntity, cancellationToken);

        _logger.LogInformation("Entity of type {EntityType} with id {Id} successfully updated", typeof(TEntity).Name, id);
    }

    public virtual async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting entity of type {EntityType} with id {Id}", typeof(TEntity).Name, id);

        var entity = await _dbcontext.Set<TEntity>().FindAsync([id], cancellationToken);
        if (entity is null)
        {
            _logger.LogWarning("Entity of type {EntityType} with id {Id} not found for delete", typeof(TEntity).Name, id);
            throw new EntityNotFoundException(nameof(TEntity), id);
        }

        _dbcontext.Set<TEntity>().Remove(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entity of type {EntityType} with id {Id} successfully deleted", typeof(TEntity).Name, id);
    }

    protected abstract void UpdateValues(TEntity oldEntity, TEntity newEntity);
}
