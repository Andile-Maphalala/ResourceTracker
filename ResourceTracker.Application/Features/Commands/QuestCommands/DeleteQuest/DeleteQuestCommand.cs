using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest
{
    public class DeleteQuestCommand : ICommand
    {
        public int Id { get; set; }
    }
}
