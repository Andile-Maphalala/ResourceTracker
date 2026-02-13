using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.DeleteBuildPlanQuest
{
    public record DeleteBuildPlanQuestCommand(int BuildPlanId, int QuestId) : ICommand;
}
