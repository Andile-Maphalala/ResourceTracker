

using MediatR;

namespace ResourceTracker.Application.Features.Queries.InventoryQueries.GetInventoryComponentQuest
{
    public record GetInventoryComponentQuestQuery(int ComponentId, int BuildPlanId) : IRequest<List<GetInventoryComponentQuestResponse>>;
}
