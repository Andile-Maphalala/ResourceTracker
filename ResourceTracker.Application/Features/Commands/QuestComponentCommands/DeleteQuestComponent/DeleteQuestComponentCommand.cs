using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponent
{
    public record DeleteQuestComponentCommand(int Id) : ICommand;
}
