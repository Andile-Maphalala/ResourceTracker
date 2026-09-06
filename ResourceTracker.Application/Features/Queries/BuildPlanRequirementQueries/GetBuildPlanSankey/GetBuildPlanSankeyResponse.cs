namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanSankey
{
    public class GetBuildPlanSankeyResponse
    {
        public int BuildPlanId { get; set; }
        public string BuildPlanName { get; set; }
        public List<SankeyEdgeDto> Edges { get; set; } = new();
        public List<ComponentFacilityDto> CraftingStations { get; set; } = new();
    }

    public class SankeyEdgeDto
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public int Value { get; set; }
    }

    public class ComponentFacilityDto
    {
        public string ComponentName { get; set; }
        public string FacilityName { get; set; }
    }
}
