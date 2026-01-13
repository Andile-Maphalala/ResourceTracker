using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Persistence.Common;
using ResourceTracker.Persistence.QueryBuilders.Interfaces;
using System.Linq.Expressions;


namespace ResourceTracker.Persistence.QueryBuilders.Implementations
{
    public class QuestComponetsQueryBuilder : IQuestComponetsQueryBuilder
    {
        public IQueryable<QuestComponents> ApplyFilters(IQueryable<QuestComponents> query, SearchQuestComponentsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(searchPredicate)
                .Where(filterPredicate);
        }

        private Expression<Func<QuestComponents, bool>> BuildFilterExpression(SearchQuestComponentsQuery request)
        {
            var predicate = PredicateBuilder.New<QuestComponents>(true);

            if (request.Id.HasValue)
                predicate = predicate.And(x => x.Id == request.Id);

            if (request.ComponentId.HasValue)
                predicate = predicate.And(x => x.ComponentId == request.ComponentId);

            if (request.Type.HasValue)
                predicate = predicate.And(x => x.Component.Type == request.Type);

            if (!string.IsNullOrEmpty(request.ComponentName))
                predicate = predicate.And(QueryableILikeExtension.ILike<QuestComponents>(x => x.Component.Name, request.ComponentName, SearchMatchType.StartsWith));

            if (!string.IsNullOrEmpty(request.ComponentDescription))
                predicate = predicate.And(QueryableILikeExtension.ILike<QuestComponents>(x => x.Component.Description, request.ComponentDescription, SearchMatchType.StartsWith));

            if (request.QuestId.HasValue)
                predicate = predicate.And(x => x.QuestId == request.QuestId);

            if (!string.IsNullOrEmpty(request.QuestName))
                predicate = predicate.And(QueryableILikeExtension.ILike<QuestComponents>(x => x.Quest.Name, request.QuestName, SearchMatchType.StartsWith));

            if (!string.IsNullOrEmpty(request.QuestDescription))
                predicate = predicate.And(QueryableILikeExtension.ILike<QuestComponents>(x => x.Quest.Description, request.QuestDescription, SearchMatchType.StartsWith));

            return predicate;
        }
        private Expression<Func<QuestComponents, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.Contains)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<QuestComponents>(true);

            var predicate = PredicateBuilder.New<QuestComponents>(false);
            

            foreach (var term in terms)
            {
                var pattern = PatternBuilder.BuildLikePattern(term, matchType);
                predicate = predicate.Or(o => EF.Functions.Like(o.Quest.Name.ToLower(), pattern.ToLower()) || EF.Functions.Like(o.Component.Name.ToLower(), pattern.ToLower()));
            }

            return predicate;
        }
    }
}

