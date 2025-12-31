using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.Search;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Domain.Entities;
using System.Linq.Expressions;

namespace ResourceTracker.Application.QueryBuilders
{
    internal static class GameQueryBuilder
    {
        internal static IQueryable<Game> ApplyFilters(this IQueryable<Game> query, SearchGamesQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(filterPredicate)
                .Where(searchPredicate);
        }

        internal static Expression<Func<Game, bool>> BuildFilterExpression(SearchGamesQuery request)
        {
            var predicate = PredicateBuilder.New<Game>(true);

            if (request.GameId.HasValue)
                predicate = predicate.And(o => o.Id == request.GameId);

            if (!string.IsNullOrEmpty(request.Name))
                predicate = predicate.And(x => x.Name.StartsWith(request.Name));

            if (!string.IsNullOrEmpty(request.Description))
                predicate = predicate.And(x => x.Description.StartsWith(request.Description));

            return predicate;
        }

        internal static Expression<Func<Game, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.Contains)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<Game>(true);

            var predicate = PredicateBuilder.New<Game>(false);
            

            foreach (var term in terms)
            {
                var pattern = PatternBuilder.BuildLikePattern(term, matchType);
                predicate = predicate.And(o => EF.Functions.Like(o.Name.ToLower(), pattern.ToLower()) || EF.Functions.Like(o.Description.ToLower(), pattern.ToLower()));
            }
            return predicate;
        }
    }
}
