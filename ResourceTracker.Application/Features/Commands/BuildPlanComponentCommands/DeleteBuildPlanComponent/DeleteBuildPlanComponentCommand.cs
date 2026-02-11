
using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponent
{
    public record DeleteBuildPlanComponentCommand(int Id) : ICommand;
}
