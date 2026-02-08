using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponents
{
    public class CreateQuestComponentsCommand : ICommand<CreateQuestComponentsResponse>
    {
        public int QuestId { get; set; }
        public List<CreateQuestComponentDto> Commands { get; set; }
    }

    public class CreateQuestComponentDto
    {
        public int AmountAquired { get; set; }

        public int ComponentId { get; set; }
    }
}
