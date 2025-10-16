using System.Linq.Expressions;
using CampusLeague.Domain.Params;

namespace CampusLeague.Application.Extensions;

public static class FilterExtensions
{
    public static IQueryable<TEntity> Filter<TEntity>(
        this IQueryable<TEntity> query, FilterParams filterParams)
        where TEntity : class
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "x");
        Expression constantExpression = Expression.Constant(true);

        if (!string.IsNullOrWhiteSpace(filterParams.Name))
        {
            MemberExpression memberExpression = Expression.Property(parameterExpression, "Name");
            Expression constantExpression1 = Expression.Constant(filterParams.Name);
            BinaryExpression binaryExpression = Expression.Equal(memberExpression, constantExpression1);

            constantExpression = Expression.AndAlso(constantExpression, binaryExpression);
        }

        var expression = Expression.Lambda<Func<TEntity, bool>>(constantExpression, parameterExpression);

        return query.Where(expression);
    }
}
