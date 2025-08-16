
using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.GameSaveCommands.UpdateGameSave
{
    public class UpdateGameSaveCommand : ICommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
