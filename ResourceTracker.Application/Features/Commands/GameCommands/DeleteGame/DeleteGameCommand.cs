using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame
{
    public class DeleteGameCommand : ICommand
    {
        public int Id { get; set; }
    }
}
