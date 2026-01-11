using LinqKit;
using ResourceTracker.Application.Models.Enums;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ResourceTracker.Persistence.Common
{
    public static class QueryableILikeExtension
    {
        public static Expression<Func<T, bool>> ILike<T>(Expression<Func<T, string>> property, string value, SearchMatchType matchType = SearchMatchType.Contains)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return PredicateBuilder.New<T>(true);
            }

            var param = property.Parameters[0];

            var pattern = PatternBuilder.BuildLikePattern(value, matchType);

            var likeMethod = typeof(NpgsqlDbFunctionsExtensions)
                .GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike),
                new[] {typeof(DbFunctions), typeof(string), typeof(string)}
                );

            var call = Expression.Call(
                likeMethod!,
                Expression.Constant(EF.Functions),
                property.Body,
                Expression.Constant(pattern)
            );

            return Expression.Lambda<Func<T, bool>>(call, param);
        }
    }
}
