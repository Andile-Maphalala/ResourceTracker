using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Persistence.Common;
using ResourceTracker.Persistence.QueryBuilders.Interfaces;
using System.Linq.Expressions;
using Component = ResourceTracker.Domain.Entities.Component;

namespace ResourceTracker.Persistence.QueryBuilders.Implementations
{
    public class ComponetQueryBuilder : IComponetQueryBuilder
    {
        public IQueryable<Component> ApplyFilters(IQueryable<Component> query, SearchComponentsQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(searchPredicate)
                .Where(filterPredicate);
        }

        private Expression<Func<Component, bool>> BuildFilterExpression(SearchComponentsQuery request)
        {
            var predicate = PredicateBuilder.New<Component>(true);

            if (request.ComponentId.HasValue)
                predicate = predicate.And(x => x.Id == request.ComponentId);

            if (request.Type.HasValue)
                predicate = predicate.And(x => x.Type == request.Type);

            if (!string.IsNullOrEmpty(request.Name))
                predicate = predicate.And(QueryableILikeExtension.ILike<Component>(x => x.Name, request.Name, SearchMatchType.StartsWith));

            if (!string.IsNullOrEmpty(request.Description))
                predicate = predicate.And(QueryableILikeExtension.ILike<Component>(x => x.Description, request.Description, SearchMatchType.StartsWith));


            return predicate;
        }
        private Expression<Func<Component, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.StartsWith)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<Component>(true);

            var predicate = PredicateBuilder.New<Component>(false);
            

            foreach (var term in terms)
            {
                var pattern = PatternBuilder.BuildLikePattern(term, matchType);
                predicate = predicate.Or(o => EF.Functions.Like(o.Name.ToLower(), pattern.ToLower()) || EF.Functions.Like(o.Description.ToLower(), pattern.ToLower()));
            }

            return predicate;
        }
    }
}
