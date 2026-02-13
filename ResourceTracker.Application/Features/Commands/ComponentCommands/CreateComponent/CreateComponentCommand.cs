using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.CreateComponent
{
    public class CreateComponentCommand : ICommand<CreateComponentResponse>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int GameId { get; set; }
    }
}
