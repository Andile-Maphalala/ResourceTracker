using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent
{
    public class CreateQuestComponentCommand : ICommand<CreateQuestComponentResponse>
    {
        public int AmountAquired { get; set; }

        public int ComponentId { get; set; }

        public int QuestId { get; set; }
    }
}
