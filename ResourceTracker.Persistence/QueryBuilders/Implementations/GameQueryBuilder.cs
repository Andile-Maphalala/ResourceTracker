using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Persistence.Common;
using ResourceTracker.Persistence.QueryBuilders.Interfaces;
using System.Linq.Expressions;

namespace ResourceTracker.Persistence.QueryBuilders.Implementations
{
    public class GameQueryBuilder : IGameQueryBuilder
    {
        public IQueryable<Game> ApplyFilters(IQueryable<Game> query, SearchGamesQuery request) 
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(filterPredicate)
                .Where(searchPredicate);
        }

        private Expression<Func<Game, bool>> BuildFilterExpression(SearchGamesQuery request)
        {
            var predicate = PredicateBuilder.New<Game>(true);

            if (request.GameId.HasValue)
                predicate = predicate.And(o => o.Id == request.GameId);

            if (!string.IsNullOrEmpty(request.Name))
            {
                predicate = predicate.And(QueryableILikeExtension.ILike<Game>(x => x.Name, request.Name, SearchMatchType.StartsWith));

            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                predicate = predicate.And(QueryableILikeExtension.ILike<Game>(x => x.Description, request.Description, SearchMatchType.StartsWith));
            }

            return predicate;
        }

        private Expression<Func<Game, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.Contains)
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
