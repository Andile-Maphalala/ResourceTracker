
namespace ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.GetBuildPlanComponent
{
    public class GetBuildPlanComponentResponse
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int QuantityNeeded { get; set; }
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int ComponentType { get; set; }
        public string ComponentTypeName { get; set; }
        public string? ComponentImageUrl { get; set; }
        public int BuildPlanId { get; set; }
        public string BuildPlanName { get; set; }
    }
}
