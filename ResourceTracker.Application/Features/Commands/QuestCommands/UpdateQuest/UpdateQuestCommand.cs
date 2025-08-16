using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.UpdateQuest
{
    public class UpdateQuestCommand : ICommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int GameSaveId { get; set; }

    }
}
