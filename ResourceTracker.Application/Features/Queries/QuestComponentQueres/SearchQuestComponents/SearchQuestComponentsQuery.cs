using MediatR;
using Pagination.Models;


namespace ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent
{
    public class SearchQuestComponentsQuery : PageableRequest, IRequest<PageableResponse<SearchQuestComponentsResponse>>
    {
        public int? Id { get; set; }
        public int? QuestId { get; set; }
        public string? QuestName { get; set; }
        public string? QuestDescription { get; set; }
        public int? ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? ComponentDescription { get; set; }
        public int? Type { get; set; }
        public string? SearchTerms { get; set; }
    }
}
