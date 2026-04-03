using System.Linq.Expressions;
using PolytechLeague.Domain.Params;

namespace PolytechLeague.Application.Extensions;

public static class FilterExtensions
{
    public static IQueryable<TEntity> Filter<TEntity>(
        this IQueryable<TEntity> query, FilterParams? filterParams)
        where TEntity : class
    {
        if (filterParams == null) return query;

        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "x");
        Expression constantExpression = Expression.Constant(true);

        if (!string.IsNullOrWhiteSpace(filterParams.Name))
        {
            var propertyInfo = typeof(TEntity).GetProperty("Name");
            if (propertyInfo != null)
            {
                MemberExpression memberExpression = Expression.Property(parameterExpression, propertyInfo);
                Expression constantExpression1 = Expression.Constant(filterParams.Name);
                BinaryExpression binaryExpression = Expression.Equal(memberExpression, constantExpression1);

                constantExpression = Expression.AndAlso(constantExpression, binaryExpression);
            }
        }

        var expression = Expression.Lambda<Func<TEntity, bool>>(constantExpression, parameterExpression);

        return query.Where(expression);
    }
}
