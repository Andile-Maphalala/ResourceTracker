using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Persistence.Common;
using ResourceTracker.Persistence.QueryBuilders.Interfaces;
using System.Linq.Expressions;

namespace ResourceTracker.Persistence.QueryBuilders.Implementations
{
    public class QuestQueryBuilder : IQuestQueryBuilder
    {
        public IQueryable<Quest> ApplyFilters(IQueryable<Quest> query, SearchQuestsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(filterPredicate)
                .Where(searchPredicate);
        }

        private Expression<Func<Quest, bool>> BuildFilterExpression(SearchQuestsQuery request)
        {
            var predicate = PredicateBuilder.New<Quest>(true);

            if (request.QuestId.HasValue)
                predicate = predicate.And(o => o.Id == request.QuestId);

            if (!string.IsNullOrEmpty(request.Name))
                predicate = predicate.And(QueryableILikeExtension.ILike<Quest>(x => x.Name, request.Name, SearchMatchType.StartsWith));

            if (!string.IsNullOrEmpty(request.Description))
                predicate = predicate.And(QueryableILikeExtension.ILike<Quest>(x => x.Description, request.Description, SearchMatchType.StartsWith));

            if (!string.IsNullOrEmpty(request.Location))
                predicate = predicate.And(QueryableILikeExtension.ILike<Quest>(x => x.Location, request.Location, SearchMatchType.StartsWith));

            if (request.GameId.HasValue)
                predicate = predicate.And(x => x.GameSave.GameId == request.GameId);

            if (request.GameSaveId.HasValue)
                predicate = predicate.And(x => x.GameSaveId == request.GameSaveId);

            if (!string.IsNullOrEmpty(request.GameName))
                predicate = predicate.And(QueryableILikeExtension.ILike<Quest>(x => x.GameSave.Game.Name, request.GameName, SearchMatchType.StartsWith));


            return predicate;
        }

        private Expression<Func<Quest, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.Contains)
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
