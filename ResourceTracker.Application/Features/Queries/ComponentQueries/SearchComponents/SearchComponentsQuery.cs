using MediatR;
using Pagination.Models;

namespace ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents
{
    public class SearchComponentsQuery : PageableSearchRequest, IRequest<PageableResponse<SearchComponentsResponse>>
    {
        public int? ComponentId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Type { get; set; }
    }
}
