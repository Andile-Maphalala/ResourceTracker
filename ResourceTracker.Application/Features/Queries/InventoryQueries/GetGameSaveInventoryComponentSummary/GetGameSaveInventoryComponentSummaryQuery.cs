

using MediatR;

namespace ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventoryTotals
{
    public record GetGameSaveInventoryComponentSummaryQuery(int GameSaveId) : IRequest<List<GetGameSaveInventoryComponentSummaryResponse>>;
}
