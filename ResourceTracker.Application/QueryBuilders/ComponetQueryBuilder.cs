using LinqKit;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Enums;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using Component = ResourceTracker.Domain.Entities.Component;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.QueryBuilders
{
    internal static class ComponetQueryBuilder
    {
        internal static IQueryable<Component> ApplyFilters(this IQueryable<Component> query, SearchComponentsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(searchPredicate)
                .Where(filterPredicate);
        }

        internal static Expression<Func<Component, bool>> BuildFilterExpression(SearchComponentsQuery request)
        {
            var predicate = PredicateBuilder.New<Component>(true);

            if (request.ComponentId.HasValue)
                predicate = predicate.And(x => x.Id == request.ComponentId);

            if (request.Type.HasValue)
                predicate = predicate.And(x => x.Type == request.Type);

            if (!string.IsNullOrEmpty(request.Name))
                predicate = predicate.And(x => x.Name.StartsWith(request.Name));

            if (!string.IsNullOrEmpty(request.Description))
                predicate = predicate.And(x => x.Description.StartsWith(request.Description));


            return predicate;
        }
        internal static Expression<Func<Component, bool>> BuildSearchExpression(IEnumerable<string> terms, ExpressionMatchTypeEnum matchType = ExpressionMatchTypeEnum.StartsWith)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<Component>(true);

            var predicate = PredicateBuilder.New<Component>(false);
            var patternFormat = PageableExtensions.GetSearchPatternFormat(matchType);

            foreach (var term in terms)
            {
                var pattern = term.BuildSearchPattern(patternFormat);
                predicate = predicate.Or(x => EF.Functions.Like(x.Name, pattern) || EF.Functions.Like(x.Description, pattern));
            }

            return predicate;
        }
    }
}
