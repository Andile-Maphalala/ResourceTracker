


using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.GameSaveCommands.CreateGameSave
{
    public class CreateGameSaveCommand : ICommand<CreateGameSaveResponse>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int GameId { get; set; }
    }
}
