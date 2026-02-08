using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponent;


namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponents
{
    public class UpdateBuildPlanComponentsCommand : ICommand
    {
        public List<UpdateBuildPlanComponentCommand> Commands { get; set; }
    }
}
