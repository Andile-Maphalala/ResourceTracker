using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Domain.Entities;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Persistence.Common;

namespace ResourceTracker.Persistence.QueryBuilders.Implementations
{
    internal static class QuestQueryBuilder
    {
        internal static IQueryable<Quest> ApplyFilters( this IQueryable<Quest> query, SearchQuestsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(filterPredicate)
                .Where(searchPredicate);
        }

        internal static Expression<Func<Quest, bool>> BuildFilterExpression(SearchQuestsQuery request)
        {
            var predicate = PredicateBuilder.New<Quest>(true);

            if (request.QuestId.HasValue)
                predicate = predicate.And(o => o.Id == request.QuestId);

            if (!string.IsNullOrEmpty(request.Name))
                predicate = predicate.And(x => EF.Functions.Like(x.Name, PatternBuilder.BuildLikePattern(request.Name, SearchMatchType.StartsWith)));

            if (!string.IsNullOrEmpty(request.Description))
                predicate = predicate.And(x => EF.Functions.Like(x.Description, PatternBuilder.BuildLikePattern(request.Description, SearchMatchType.StartsWith)));

            if (!string.IsNullOrEmpty(request.Location))
                predicate = predicate.And(x => EF.Functions.Like(x.Location, PatternBuilder.BuildLikePattern(request.Location, SearchMatchType.StartsWith)));

            if (request.GameId.HasValue)
                predicate = predicate.And(x => x.GameSave.GameId == request.GameId);

            if(!string.IsNullOrEmpty(request.GameName))
                predicate = predicate.And(x => EF.Functions.Like(x.GameSave.Game.Name, PatternBuilder.BuildLikePattern(request.GameName, SearchMatchType.StartsWith)));


            return predicate;
        }

        internal static Expression<Func<Quest, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.StartsWith)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<Quest>(true);

            var predicate = PredicateBuilder.New<Quest>(false);
            

            foreach (var term in terms)
            {
                var pattern = PatternBuilder.BuildLikePattern(term, matchType);
                predicate = predicate.And(o => EF.Functions.Like(o.Name.ToLower(), pattern.ToLower()) || EF.Functions.Like(o.Description.ToLower(), pattern.ToLower()) || EF.Functions.Like(o.Location.ToLower(), pattern.ToLower()));
            }
            return predicate;
        }
    }
}
