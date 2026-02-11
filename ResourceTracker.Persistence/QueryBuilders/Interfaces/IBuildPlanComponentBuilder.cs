
using ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.SearchBuildPlanComponent;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.QueryBuilders.Interfaces
{
    public interface IBuildPlanComponentBuilder
    {
        IQueryable<BuildPlanComponent> ApplyFilters(IQueryable<BuildPlanComponent> query, SearchBuildPlanComponentQuery request);
    }
}
