using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using Component = ResourceTracker.Domain.Entities.Component;
using System.Linq.Expressions;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Persistence.Common;

namespace ResourceTracker.Persistence.QueryBuilders.Implementations
{
    internal static class ComponetQueryBuilder
    {
        internal static IQueryable<Component> ApplyFilters(this IQueryable<Component> query, SearchComponentsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
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
                predicate = predicate.And(x => EF.Functions.Like(x.Name, PatternBuilder.BuildLikePattern(request.Name, SearchMatchType.StartsWith)));

            if (!string.IsNullOrEmpty(request.Description))
                predicate = predicate.And(x => EF.Functions.Like(x.Description, PatternBuilder.BuildLikePattern(request.Description, SearchMatchType.StartsWith)));


            return predicate;
        }
        internal static Expression<Func<Component, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.StartsWith)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<Component>(true);

            var predicate = PredicateBuilder.New<Component>(false);
            

            foreach (var term in terms)
            {
                var pattern = PatternBuilder.BuildLikePattern(term, matchType);
                predicate = predicate.Or(x => EF.Functions.Like(x.Name.ToLower(), pattern.ToLower()) || EF.Functions.Like(x.Description.ToLower(), pattern.ToLower()));
            }

            return predicate;
        }
    }
}
