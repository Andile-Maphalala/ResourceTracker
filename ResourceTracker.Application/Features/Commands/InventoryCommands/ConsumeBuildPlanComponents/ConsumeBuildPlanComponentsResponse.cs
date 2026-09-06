

namespace ResourceTracker.Application.Features.Commands.InventoryCommands.ConsumeBuildPlanComponents
{
    public class ConsumeBuildPlanComponentsResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
