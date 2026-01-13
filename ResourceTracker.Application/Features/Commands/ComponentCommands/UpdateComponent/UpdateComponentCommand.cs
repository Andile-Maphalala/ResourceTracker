using MediatR;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent
{
    public class UpdateComponentCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
    }
}
