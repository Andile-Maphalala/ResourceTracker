using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;
using ResourceTracker.Domain.Entities;


namespace ResourceTracker.Persistence.QueryBuilders.Interfaces
{
    public interface IGameQueryBuilder
    {
        IQueryable<Game> ApplyFilters(IQueryable<Game> query, SearchGamesQuery request);
    }
}
