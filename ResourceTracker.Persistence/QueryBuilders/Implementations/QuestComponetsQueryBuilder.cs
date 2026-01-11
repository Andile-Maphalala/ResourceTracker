using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Domain.Entities;
using System.Linq.Expressions;
using ResourceTracker.Persistence.Common;


namespace ResourceTracker.Persistence.QueryBuilders.Implementations
{
    internal static class QuestComponetsQueryBuilder
    {
        internal static IQueryable<QuestComponents> ApplyFilters(this IQueryable<QuestComponents> query, SearchQuestComponentsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(searchPredicate)
                .Where(filterPredicate);
        }

        internal static Expression<Func<QuestComponents, bool>> BuildFilterExpression(SearchQuestComponentsQuery request)
        {
            var predicate = PredicateBuilder.New<QuestComponents>(true);

            if (request.Id.HasValue)
                predicate = predicate.And(x => x.Id == request.Id);

            if (request.ComponentId.HasValue)
                predicate = predicate.And(x => x.ComponentId == request.ComponentId);

            if (request.Type.HasValue)
                predicate = predicate.And(x => x.Component.Type == request.Type);

            if (!string.IsNullOrEmpty(request.ComponentName))
                predicate = predicate.And(x => EF.Functions.Like(x.Component.Name, PatternBuilder.BuildLikePattern(request.ComponentName, SearchMatchType.StartsWith)));

            if (!string.IsNullOrEmpty(request.ComponentDescription))
                predicate = predicate.And(x => EF.Functions.Like(x.Component.Description, PatternBuilder.BuildLikePattern(request.ComponentDescription, SearchMatchType.StartsWith)));

            if (request.QuestId.HasValue)
                predicate = predicate.And(x => x.QuestId == request.QuestId);

            if (!string.IsNullOrEmpty(request.QuestName))
                predicate = predicate.And(x => EF.Functions.Like(x.Quest.Name, PatternBuilder.BuildLikePattern(request.QuestName, SearchMatchType.StartsWith)));

            if (!string.IsNullOrEmpty(request.QuestDescription))
                predicate = predicate.And(x => EF.Functions.Like(x.Quest.Description, PatternBuilder.BuildLikePattern(request.QuestDescription, SearchMatchType.StartsWith)));

            return predicate;
        }
        internal static Expression<Func<QuestComponents, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.StartsWith)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<QuestComponents>(true);

            var predicate = PredicateBuilder.New<QuestComponents>(false);
            

            foreach (var term in terms)
            {
                var pattern = PatternBuilder.BuildLikePattern(term, matchType);
                predicate = predicate.Or(x => EF.Functions.Like(x.Quest.Name.ToLower(), pattern.ToLower()) || EF.Functions.Like(x.Component.Name.ToLower(), pattern.ToLower()));
            }

            return predicate;
        }
    }
}

