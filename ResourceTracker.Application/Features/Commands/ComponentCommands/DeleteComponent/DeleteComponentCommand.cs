
using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent
{
    public record DeleteComponentCommand(int Id) : ICommand;
}
