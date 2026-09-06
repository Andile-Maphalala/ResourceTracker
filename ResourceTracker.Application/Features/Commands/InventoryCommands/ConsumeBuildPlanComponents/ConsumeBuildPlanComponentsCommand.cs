

using MediatR;

namespace ResourceTracker.Application.Features.Commands.InventoryCommands.ConsumeBuildPlanComponents
{
    public record ConsumeBuildPlanComponentsCommand(List<ConsumeAllocation> Allocations) : IRequest<ConsumeBuildPlanComponentsResponse>;

    public class ConsumeAllocation
    {
        public int QuestComponentId { get; set; }
        public int AmountToDeduct { get; set; }
    }
}
