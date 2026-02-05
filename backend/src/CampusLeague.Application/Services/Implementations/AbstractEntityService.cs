using CampusLeague.Application.Extensions;
using CampusLeague.Domain.Exceptions;
using CampusLeague.Application.Services.Interfaces;
using CampusLeague.Domain.Params;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CampusLeague.Persistence.Database;

namespace CampusLeague.Application.Services.Implementations;

public abstract class AbstractEntityService<TEntity>(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<TEntity>> _logger) 
    : IAbstractEntityService<TEntity> where TEntity : class
{
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        FilterParams? filterParams = null,
        SortParams? sortParams = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching all entities of type {EntityType}", typeof(TEntity).Name);

        var entities = await _dbcontext.Set<TEntity>()
            .AsNoTracking()
            .Filter(filterParams!)
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
            .Filter(filterParams!)
            .Sort(sortParams)
            .Page(pageParams, cancellationToken);

        _logger.LogInformation("Fetched {Count} paged entities of type {EntityType}", entities.Data.Count, typeof(TEntity).Name);

        return entities;
    }

    public virtual async Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching entity of type {EntityType} with id {Id}", typeof(TEntity).Name, id);

        var entity = await _dbcontext.Set<TEntity>().FindAsync(id, cancellationToken);
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

        _logger.LogInformation("Entity of type {EntityType} successfully created", typeof(TEntity).Name);
        return entity;
    }

    public virtual async Task UpdateAsync(
        long id, TEntity newEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating entity of type {EntityType} with id {Id}", typeof(TEntity).Name, id);

        var oldEntity = await _dbcontext.Set<TEntity>().FindAsync(id, cancellationToken);
        if (oldEntity is null)
        {
            _logger.LogWarning("Entity of type {EntityType} with id {Id} not found for update", typeof(TEntity).Name, id);
            throw new EntityNotFoundException(nameof(TEntity), id);
        }

        UpdateValues(oldEntity, newEntity);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entity of type {EntityType} with id {Id} successfully updated", typeof(TEntity).Name, id);
    }

    public virtual async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting entity of type {EntityType} with id {Id}", typeof(TEntity).Name, id);

        var entity = await _dbcontext.Set<TEntity>().FindAsync(id, cancellationToken);
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
