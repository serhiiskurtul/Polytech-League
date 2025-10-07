using CampusLeague.Domain.Params;
using Microsoft.EntityFrameworkCore;

namespace CampusLeague.Application.Extensions;


/// Applies pagination to the query. Ensure you use OrderBy() before calling this method.
public static class PageExtensions
{
    public static async Task<PageResult<TEntity>> Page<TEntity>(
        this IQueryable<TEntity> query,
        PageParams? pageParams = null,
        CancellationToken cancellationToken = default)
    {
        var count = await query.CountAsync(cancellationToken);
        if (count == 0) return new PageResult<TEntity>([], 0);

        var page = pageParams!.Page ?? 1;
        var pageSize = pageParams.PageSize ?? 10;

        var skip = (page - 1) * pageSize;
        var result = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<TEntity>(result, count);
    }
}
