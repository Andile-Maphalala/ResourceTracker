using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.DeletePlayerFacility
{
    public record DeletePlayerFacilityCommand(int Id) : ICommand;
}
