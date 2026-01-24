

using MediatR;

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement
{
    public class GetBuildPlanRequirementQuery : IRequest<GetBuildPlanRequirementsResponse>
    {
        public int BuildPlanId { get; set; }
        public bool IgnoreFacilityRequirements { get; set; }
        public bool IgnoreInventory{ get; set; }
    }
}
