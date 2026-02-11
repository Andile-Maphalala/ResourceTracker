using ResourceTracker.Application.Common.CQRS;


namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponents
{
    public class CreateBuildPlanComponentsCommand : ICommand<CreateBuildPlanComponentsResponse>
    {
        public int BuildPlanId { get; set; }
        public List<CreateBuildPlanComponentDto> Commands { get; set; }
    }

    public class CreateBuildPlanComponentDto
    {
        public int Order { get; set; }
        public int QuantityNeeded { get; set; }
        public int ComponentId { get; set; }
    }
}
