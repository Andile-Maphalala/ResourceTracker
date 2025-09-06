
using Microsoft.AspNetCore.Http;
using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.ImportCommands.ImportGame
{
    public class ImportGameCommand : ICommand<ImportGameResponse>
    {
        public int GameId { get; set; }
        public IFormFile File { get; set; }
    }
}
