using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponents
{
    public class CreateQuestComponentsCommand : ICommand<CreateQuestComponentsResponse>
    {
        public int QuestId { get; set; }
        public List<CreateQuestComponentClass> Commands { get; set; }
    }

    public class CreateQuestComponentClass
    {
        public int AmountAquired { get; set; }

        public int ComponentId { get; set; }
    }
}
