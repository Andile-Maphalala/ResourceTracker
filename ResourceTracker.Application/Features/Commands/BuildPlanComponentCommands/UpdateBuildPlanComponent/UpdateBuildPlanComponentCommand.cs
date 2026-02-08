

using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponent
{
    public class UpdateBuildPlanComponentCommand : ICommand
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int QuantityNeeded { get; set; }
    }
}
