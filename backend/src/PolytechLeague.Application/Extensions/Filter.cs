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
            else
            {
                // Fallback for entities without 'Name' property, like 'User'
                // Search in FirstName, LastName, or Email
                var firstNameProp = typeof(TEntity).GetProperty("FirstName");
                var lastNameProp = typeof(TEntity).GetProperty("LastName");
                var emailProp = typeof(TEntity).GetProperty("Email");

                Expression orExpression = Expression.Constant(false);
                var filterValue = Expression.Constant(filterParams.Name);

                if (firstNameProp != null)
                {
                    var prop = Expression.Property(parameterExpression, firstNameProp);
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var call = Expression.Call(prop, contains, filterValue);
                    orExpression = Expression.OrElse(orExpression, call);
                }
                if (lastNameProp != null)
                {
                    var prop = Expression.Property(parameterExpression, lastNameProp);
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var call = Expression.Call(prop, contains, filterValue);
                    orExpression = Expression.OrElse(orExpression, call);
                }
                if (emailProp != null)
                {
                    var prop = Expression.Property(parameterExpression, emailProp);
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var call = Expression.Call(prop, contains, filterValue);
                    orExpression = Expression.OrElse(orExpression, call);
                }

                constantExpression = Expression.AndAlso(constantExpression, orExpression);
            }
        }

        var expression = Expression.Lambda<Func<TEntity, bool>>(constantExpression, parameterExpression);

        return query.Where(expression);
    }
}
