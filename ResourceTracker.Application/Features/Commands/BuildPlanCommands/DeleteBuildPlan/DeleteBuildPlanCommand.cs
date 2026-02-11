using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.BuildPlanCommands.DeleteBuildPlan
{
    public record DeleteBuildPlanCommand(int Id) : ICommand;
}
