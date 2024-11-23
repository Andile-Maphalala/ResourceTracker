using MediatR;
using Pagination.Models;

namespace ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests
{
    public class SearchQuestsQuery : PageableSearchRequest, IRequest<PageableResponse<SearchQuestsResponse>>
    {
        public int? QuestId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
