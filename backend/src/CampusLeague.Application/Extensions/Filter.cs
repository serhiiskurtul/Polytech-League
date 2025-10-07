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

        if (filterParams.MinPrice is not null)
        {
            MemberExpression memberExpressionMinPrice = Expression.Property(parameterExpression, "MinPrice");
            Expression constantExpressionMinPrice = Expression.Constant(filterParams.MinPrice);
            BinaryExpression binaryExpression = Expression.GreaterThanOrEqual(memberExpressionMinPrice, constantExpressionMinPrice);

            constantExpression = Expression.AndAlso(constantExpression, binaryExpression);
        }

        if (filterParams.MaxPrice is not null)
        {
            MemberExpression memberExpressionMaxPrice = Expression.Property(parameterExpression, "MaxPrice");
            Expression constantExpressionMaxPrice = Expression.Constant(filterParams.MaxPrice);
            BinaryExpression binaryExpression = Expression.LessThanOrEqual(memberExpressionMaxPrice, constantExpressionMaxPrice); 

            constantExpression = Expression.AndAlso(constantExpression, binaryExpression);
        }

    
        var expression = Expression.Lambda<Func<TEntity, bool>>(constantExpression, parameterExpression);

        return query.Where(expression);
    }
}
