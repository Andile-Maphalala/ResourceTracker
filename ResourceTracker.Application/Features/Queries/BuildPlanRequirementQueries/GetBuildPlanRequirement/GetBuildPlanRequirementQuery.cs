

using MediatR;

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement
{
    public class GetBuildPlanRequirementQuery : IRequest<GetBuildPlanRequirementsResponse>
    {
        public int BuildPlanId { get; set; }
        public bool IncludeFacilityRequirements { get; set; }
        public bool IncludeInventory{ get; set; }
    }
}
