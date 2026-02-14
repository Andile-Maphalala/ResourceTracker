

namespace ResourceTracker.Application.Features.Queries.GameQueries.SearchGames
{
    public class SearchGamesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
