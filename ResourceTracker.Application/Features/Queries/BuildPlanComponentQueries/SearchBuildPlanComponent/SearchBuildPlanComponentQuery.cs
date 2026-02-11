
using MediatR;
using Pagination.Models;

namespace ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.SearchBuildPlanComponent
{
    public class SearchBuildPlanComponentQuery : PageableRequest, IRequest<PageableResponse<SearchBuildPlanComponentResponse>>
    {
        public int? Id { get; set; }
        public int? BuildPlanId { get; set; }
        public string? BuildPlanName { get; set; }
        public string? BuildPlanDescription { get; set; }
        public int? ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? ComponentDescription { get; set; }
        public int? Type { get; set; }
        public string? SearchTerms { get; set; }
    }
}
