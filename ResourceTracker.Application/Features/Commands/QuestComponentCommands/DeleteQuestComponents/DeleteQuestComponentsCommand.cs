
using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponents
{
    public class DeleteQuestComponentsCommand : ICommand
    {
        public List<int> Ids { get; set; }
    }
}
