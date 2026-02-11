using MediatR;

namespace ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.GetBuildPlanComponent
{
    public record GetBuildPlanComponentQuery(int Id) : IRequest<GetBuildPlanComponentResponse>;
}
