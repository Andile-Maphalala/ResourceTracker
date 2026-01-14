using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame
{
    public record DeleteGameCommand(int Id) : ICommand;
}
