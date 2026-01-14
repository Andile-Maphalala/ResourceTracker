using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest
{
    public record DeleteQuestCommand(int Id) : ICommand;
}
