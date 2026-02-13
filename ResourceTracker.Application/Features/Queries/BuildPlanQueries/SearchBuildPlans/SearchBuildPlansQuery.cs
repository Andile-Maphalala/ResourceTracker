using MediatR;
using Pagination.Models;

namespace ResourceTracker.Application.Features.Queries.BuildPlanQueries.SearchBuildPlans
{
    public class SearchBuildPlansQuery : PageableRequest, IRequest<PageableResponse<SearchBuildPlansResponse>>
    {
        public int? BuildPlanId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? GameId { get; set; }
        public string? GameName { get; set; }
        public int? GameSaveId { get; set; }
        public string? SearchTerms { get; set; }
    }
}
