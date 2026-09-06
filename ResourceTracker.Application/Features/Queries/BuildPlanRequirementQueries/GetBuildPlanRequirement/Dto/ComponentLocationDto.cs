

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement.Dto
{
    public class ComponentLocationDto
    {
        public int QuestComponentId { get; set; }
        public int QuestId { get; set; }
        public string QuestName { get; set; }
        public int Quantity { get; set; }
    }
}
