

using MediatR;

namespace ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventorySummary
{
    public record GetGameSaveInventorySummaryQuery(int GameSaveId) : IRequest<GetGameSaveInventorySummaryResponse>;
}
