using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.BuildPlanCommands.CreateBuildPlan
{
    public class CreateBuildPlanCommand : ICommand<CreateBuildPlanResponse>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int GameSaveId { get; set; }
        public bool IncludeInventory { get; set; }
        public bool IncludeFacilityRequirements { get; set; }
    }
}
