using System.Linq.Expressions;

namespace FinancesBlazor.Extensions;

public static class LinqExtensions
{
    public static IQueryable<T> OrderByDynamic<T>(this List<T> query, string attribute, bool descending)
    {
        var queryable = query.AsQueryable();
        try
        {
            string orderMethodName = descending ? "OrderByDescending" : "OrderBy";
            Type t = typeof(T);

            var param = Expression.Parameter(t);
            var property = t.GetProperty(attribute);
            return queryable.Provider.CreateQuery<T>(
                Expression.Call(
                    typeof(Queryable),
                    orderMethodName,
                    new Type[] { t, property.PropertyType },
                    queryable.Expression,
                    Expression.Quote(
                        Expression.Lambda(
                            Expression.Property(param, property),
                            param))
                ));
        }
        catch (Exception) // Probably invalid input, you can catch specifics if you want
        {
            return queryable; // Return unsorted query
        }
    }
}
