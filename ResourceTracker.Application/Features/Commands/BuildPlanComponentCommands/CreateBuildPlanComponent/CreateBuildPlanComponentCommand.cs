using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponent
{
    public class CreateBuildPlanComponentCommand : ICommand<CreateBuildPlanComponentResponse>
    {
        public int Order { get; set; }
        public int QuantityNeeded { get; set; }
        public int ComponentId { get; set; }
        public int BuildPlanId { get; set; }
    }
}
