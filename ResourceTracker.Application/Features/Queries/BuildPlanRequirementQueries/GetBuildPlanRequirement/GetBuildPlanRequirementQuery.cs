

using MediatR;

namespace ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement
{
    public record GetBuildPlanRequirementQuery(int BuildPlanId) : IRequest<GetBuildPlanRequirementsResponse>;
}
