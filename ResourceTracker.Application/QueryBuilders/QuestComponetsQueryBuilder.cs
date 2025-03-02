using LinqKit;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Enums;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;
using ResourceTracker.Domain.Entities;
using System.Linq.Expressions;


namespace ResourceTracker.Application.QueryBuilders
{
    internal static class QuestComponetsQueryBuilder
    {
        internal static IQueryable<QuestComponents> ApplyFilters(this IQueryable<QuestComponents> query, SearchQuestComponentsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.GetSearchTerms());
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
                predicate = predicate.And(x => x.Component.Name.StartsWith(request.ComponentName));

            if (!string.IsNullOrEmpty(request.ComponentDescription))
                predicate = predicate.And(x => x.Component.Description.StartsWith(request.ComponentDescription));

            if (request.QuestId.HasValue)
                predicate = predicate.And(x => x.QuestId == request.QuestId);

            if (!string.IsNullOrEmpty(request.QuestName))
                predicate = predicate.And(x => x.Quest.Name.StartsWith(request.QuestName));

            if (!string.IsNullOrEmpty(request.QuestDescription))
                predicate = predicate.And(x => x.Quest.Description.StartsWith(request.QuestDescription));

            return predicate;
        }
        internal static Expression<Func<QuestComponents, bool>> BuildSearchExpression(IEnumerable<string> terms, ExpressionMatchTypeEnum matchType = ExpressionMatchTypeEnum.StartsWith)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<QuestComponents>(true);

            var predicate = PredicateBuilder.New<QuestComponents>(false);
            var patternFormat = PageableExtensions.GetSearchPatternFormat(matchType);

            foreach (var term in terms)
            {
                var pattern = term.BuildSearchPattern(patternFormat);
                predicate = predicate.Or(x => EF.Functions.Like(x.Quest.Name, pattern) || EF.Functions.Like(x.Component.Name, pattern));
            }

            return predicate;
        }
    }
}

