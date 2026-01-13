
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.QueryBuilders.Interfaces
{
    public interface IQuestQueryBuilder
    {
        IQueryable<Quest> ApplyFilters(IQueryable<Quest> query, SearchQuestsQuery request);
    }
}
