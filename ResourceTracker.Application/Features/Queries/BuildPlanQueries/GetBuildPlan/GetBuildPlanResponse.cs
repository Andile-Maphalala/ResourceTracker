namespace ResourceTracker.Application.Features.Queries.BuildPlanQueries.GetBuildPlan
{
    public class GetBuildPlanResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IncludeInventory { get; set; }
        public bool IncludeFacilityRequirements { get; set; }
        public int GameSaveId { get; set; }
        public string GameSaveName { get; set; }
    }
}
