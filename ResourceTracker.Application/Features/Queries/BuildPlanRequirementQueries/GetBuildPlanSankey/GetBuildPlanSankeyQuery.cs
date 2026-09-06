

using MediatR;

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanSankey
{
    public record GetBuildPlanSankeyQuery(int BuildPlanId) : IRequest<GetBuildPlanSankeyResponse>;
}
