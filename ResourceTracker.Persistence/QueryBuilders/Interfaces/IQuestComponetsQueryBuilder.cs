using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.QueryBuilders.Interfaces
{
    public interface IQuestComponetsQueryBuilder
    {
        IQueryable<QuestComponents> ApplyFilters(IQueryable<QuestComponents> query, SearchQuestComponentsQuery request);
    }
}
