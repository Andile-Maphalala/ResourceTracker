

using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto;

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement
{
    public class GetBuildPlanRequirementsResponse
    {
        public int BuildPlanId { get; set; }
        public string BuildPlanName { get; set; }
        public int TotalRequired { get; set; }
        public int TotalAvailable { get; set; }
        public int TotalMissing { get; set; }
        public List<BuildPlanComponentRequirementDto> Requirements { get; set; }
        public List<BuildPlanFacilityRequirementDto> FacilityRequirements { get; set; }

    }
}
