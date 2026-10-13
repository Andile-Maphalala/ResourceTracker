using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.UpdatePlayerFacility
{
    public class UpdatePlayerFacilityCommand : ICommand
    {
        public int Id { get; set; }
        public int Level { get; set; } = 1;
    }
}
