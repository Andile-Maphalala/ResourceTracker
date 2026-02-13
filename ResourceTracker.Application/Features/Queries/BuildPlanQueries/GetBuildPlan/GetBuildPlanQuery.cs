using MediatR;

namespace ResourceTracker.Application.Features.Queries.BuildPlanQueries.GetBuildPlan
{
    public record GetBuildPlanQuery(int Id) : IRequest<GetBuildPlanResponse>;
}
