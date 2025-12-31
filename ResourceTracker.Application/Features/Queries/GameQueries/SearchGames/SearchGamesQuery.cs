using MediatR;
using Pagination.Models;


namespace ResourceTracker.Application.Features.Queries.GameQueries.SearchGames
{
    public class SearchGamesQuery : PageableRequest, IRequest<PageableResponse<SearchGamesResponse>>
    {
        public int? GameId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
        public string? SearchTerms { get; set; }

    }
}
