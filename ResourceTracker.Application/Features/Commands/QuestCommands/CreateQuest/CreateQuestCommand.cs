using MediatR;
using ResourceTracker.Application.Common.CQRS;


namespace ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest
{
    public class CreateQuestCommand : ICommand<CreateQuestResponse>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int GameId { get; set; }
    }
}
