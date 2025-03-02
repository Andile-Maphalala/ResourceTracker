using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent
{
    public class UpdateQuestComponentCommand : ICommand
    {
        public int Id { get; set; }

        public int AmountRequired { get; set; }

        public int AmountAquired { get; set; }
    }
}
