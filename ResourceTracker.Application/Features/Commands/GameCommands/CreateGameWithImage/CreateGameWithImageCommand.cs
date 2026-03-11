using Microsoft.AspNetCore.Http;
using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.GameCommands.CreateGame
{
    public class CreateGameWithImageCommand : ICommand<CreateGameWithImageResponse>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? AltText { get; set; }
        public IFormFile? Image { get; set; }
    }
}
