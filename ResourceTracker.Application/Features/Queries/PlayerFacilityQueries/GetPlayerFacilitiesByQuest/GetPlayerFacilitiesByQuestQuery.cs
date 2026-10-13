using MediatR;

namespace ResourceTracker.Application.Features.Queries.PlayerFacilityQueries.GetPlayerFacilitiesByQuest
{
    public record GetPlayerFacilitiesByQuestQuery(int QuestId) : IRequest<List<GetPlayerFacilitiesByQuestResponse>>;
}
