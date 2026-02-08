
using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponents
{
    public class DeleteBuildPlanComponentsCommand : ICommand
    {
        public List<int> Ids { get; set; }
    }
}
