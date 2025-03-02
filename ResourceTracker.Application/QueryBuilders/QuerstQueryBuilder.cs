using Pagination.Enums;
using Pagination;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Domain.Entities;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
namespace ResourceTracker.Application.QueryBuilders
{
    internal static class QuerstQueryBuilder
    {
        internal static IQueryable<Quest> ApplyFilters( this IQueryable<Quest> query, SearchQuestsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.GetSearchTerms());
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
                predicate = predicate.And(x => x.Name.StartsWith(request.Name));

            if (!string.IsNullOrEmpty(request.Description))
                predicate = predicate.And(x => x.Description.StartsWith(request.Description));

            if (!string.IsNullOrEmpty(request.Location))
                predicate = predicate.And(x => x.Description.StartsWith(request.Location));


            return predicate;
        }

        internal static Expression<Func<Quest, bool>> BuildSearchExpression(IEnumerable<string> terms, ExpressionMatchTypeEnum matchType = ExpressionMatchTypeEnum.StartsWith)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<Quest>(true);

            var predicate = PredicateBuilder.New<Quest>(false);
            var patternFormat = PageableExtensions.GetSearchPatternFormat(matchType);

            foreach (var term in terms)
            {
                var pattern = term.BuildSearchPattern(patternFormat);
                predicate = predicate.And(o => EF.Functions.Like(o.Name, pattern) || EF.Functions.Like(o.Description, pattern) || EF.Functions.Like(o.Location, pattern));
            }
            return predicate;
        }
    }
}
