
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.QueryBuilders.Interfaces
{
    public interface IComponetQueryBuilder
    {
        IQueryable<Component> ApplyFilters(IQueryable<Component> query, SearchComponentsQuery request);
    }
}
