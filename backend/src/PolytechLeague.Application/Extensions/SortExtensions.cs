using PolytechLeague.Domain.Params;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.Application.Extensions;

public static class SortExtensions
{
    public static IQueryable<TEntity> Sort<TEntity>(
        this IQueryable<TEntity> query,
        SortParams? sortParams = null)
    {
        if (sortParams == null || string.IsNullOrEmpty(sortParams.OrderBy)) return query;

        if (sortParams.SortDirection == SortDirection.Desc)
        {
            return query.OrderByDescending(x => EF.Property<object>(x!, sortParams.OrderBy));
        }

        return query.OrderBy(x => EF.Property<object>(x!, sortParams.OrderBy));
    }
}
