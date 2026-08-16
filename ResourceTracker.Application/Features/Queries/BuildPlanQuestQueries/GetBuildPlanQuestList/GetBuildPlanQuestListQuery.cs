

using MediatR;

namespace ResourceTracker.Application.Features.Queries.BuildPlanQuestQueries.GetBuildPlanQuestList
{
    public record GetBuildPlanQuestListQuery(int? BuildPlanId, int? QuestId) : IRequest<List<GetBuildPlanQuestListResponse>>;
}
