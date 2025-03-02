using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponents
{
    public class UpdateQuestComponentsCommand : ICommand
    {
        public List<UpdateQuestComponentCommand> Commands { get; set; }
    }
}
