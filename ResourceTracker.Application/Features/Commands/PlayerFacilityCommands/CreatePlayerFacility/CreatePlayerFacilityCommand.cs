using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.CreatePlayerFacility
{
    public class CreatePlayerFacilityCommand : ICommand<CreatePlayerFacilityResponse>
    {
        public int ComponentId { get; set; }
        public int QuestId { get; set; }
        public int Level { get; set; } = 1;
    }
}
