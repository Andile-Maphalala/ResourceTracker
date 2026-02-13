using ResourceTracker.Application.Features.Queries.BuildPlanQueries.SearchBuildPlans;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.QueryBuilders.Interfaces
{
    public interface IBuildPlanQueryBuilder
    {
        IQueryable<BuildPlan> ApplyFilters(IQueryable<BuildPlan> query, SearchBuildPlansQuery request);
    }
}
