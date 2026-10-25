using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.GameCommands.PurgeGame
{
    public record PurgeGameCommand(int Id, string ConfirmName) : ICommand<PurgeGameResponse>;
}
