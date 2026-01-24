

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto
{
    public class BuildPlanComponentRequirementDto
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int RequiredAmount { get; set; }
        public int AvailableAmount { get; set; }
        public int MissingAmount { get; set; }
        public int Type { get; set; }
    }
}
