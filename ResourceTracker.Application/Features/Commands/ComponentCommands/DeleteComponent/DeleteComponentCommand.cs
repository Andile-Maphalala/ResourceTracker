using MediatR;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent
{
    public record DeleteComponentCommand(int Id) : IRequest;
}
