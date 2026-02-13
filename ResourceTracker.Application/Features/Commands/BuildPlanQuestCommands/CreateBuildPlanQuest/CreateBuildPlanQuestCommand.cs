using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.CreateBuildPlanQuest
{
    public class CreateBuildPlanQuestCommand : ICommand<CreateBuildPlanQuestResponse>
    {
        public int BuildPlanId { get; set; }
        public int QuestId { get; set; }
    }
}
