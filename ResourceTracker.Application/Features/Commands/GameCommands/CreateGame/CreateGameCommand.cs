using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.GameCommands.CreateGame
{
    public class CreateGameCommand : ICommand<CreateGameResponse>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int GameId { get; set; }
    }
}
