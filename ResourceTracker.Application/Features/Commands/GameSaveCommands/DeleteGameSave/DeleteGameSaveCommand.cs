using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.GameSaveCommands.DeleteGameSave
{
    public record DeleteGameSaveCommand(int Id): ICommand;
}
