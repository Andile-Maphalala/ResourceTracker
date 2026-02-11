

using LinqKit;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.SearchBuildPlanComponent;
using ResourceTracker.Application.Models.Enums;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Persistence.Common;
using ResourceTracker.Persistence.QueryBuilders.Interfaces;
using System.Linq.Expressions;

namespace ResourceTracker.Persistence.QueryBuilders.Implementations
{
    public class BuildPlanComponentBuilder : IBuildPlanComponentBuilder
    {
        public IQueryable<BuildPlanComponent> ApplyFilters(IQueryable<BuildPlanComponent> query, SearchBuildPlanComponentQuery request)
        {
            var filterPredicate = BuildFilterExpression(request);
            var searchPredicate = BuildSearchExpression(request.SearchTerms.GetSearchTerms());
            return query
                .AsExpandable()
                .Where(searchPredicate)
                .Where(filterPredicate);
        }

        private Expression<Func<BuildPlanComponent, bool>> BuildFilterExpression(SearchBuildPlanComponentQuery request)
        {
            var predicate = PredicateBuilder.New<BuildPlanComponent>(true);

            if (request.Id.HasValue)
                predicate = predicate.And(x => x.Id == request.Id);

            if (request.ComponentId.HasValue)
                predicate = predicate.And(x => x.ComponentId == request.ComponentId);

            if (request.Type.HasValue)
                predicate = predicate.And(x => x.Component.Type == request.Type);

            if (!string.IsNullOrEmpty(request.ComponentName))
                predicate = predicate.And(QueryableILikeExtension.ILike<BuildPlanComponent>(x => x.Component.Name, request.ComponentName, SearchMatchType.StartsWith));

            if (!string.IsNullOrEmpty(request.ComponentDescription))
                predicate = predicate.And(QueryableILikeExtension.ILike<BuildPlanComponent>(x => x.Component.Description, request.ComponentDescription, SearchMatchType.StartsWith));

            if (request.BuildPlanId.HasValue)
                predicate = predicate.And(x => x.BuildPlanId == request.BuildPlanId);

            if (!string.IsNullOrEmpty(request.BuildPlanName))
                predicate = predicate.And(QueryableILikeExtension.ILike<BuildPlanComponent>(x => x.BuildPlan.Name, request.BuildPlanName, SearchMatchType.StartsWith));

            if (!string.IsNullOrEmpty(request.BuildPlanDescription))
                predicate = predicate.And(QueryableILikeExtension.ILike<BuildPlanComponent>(x => x.BuildPlan.Description, request.BuildPlanDescription, SearchMatchType.StartsWith));

            return predicate;
        }
        private Expression<Func<BuildPlanComponent, bool>> BuildSearchExpression(IEnumerable<string> terms, SearchMatchType matchType = SearchMatchType.Contains)
        {
            if (terms == null || !terms.Any())
                return PredicateBuilder.New<BuildPlanComponent>(true);

            var predicate = PredicateBuilder.New<BuildPlanComponent>(false);


            foreach (var term in terms)
            {
                var pattern = PatternBuilder.BuildLikePattern(term, matchType);
                predicate = predicate.Or(o => EF.Functions.Like(o.BuildPlan.Name.ToLower(), pattern.ToLower()) || EF.Functions.Like(o.Component.Name.ToLower(), pattern.ToLower()));
            }

            return predicate;
        }
    }
}
