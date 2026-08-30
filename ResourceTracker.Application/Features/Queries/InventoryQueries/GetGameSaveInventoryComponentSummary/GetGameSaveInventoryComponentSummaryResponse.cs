

using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventoryComponentSummary.Dto;

namespace ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventoryTotals
{
    public class GetGameSaveInventoryComponentSummaryResponse
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string ComponentType { get; set; }
        public string ComponentImageUrl { get; set; }
        public int TotalQuantity { get; set; }
        public List<GetGameSaveInventoryComponentQuestDto> Quests { get; set; } = new List<GetGameSaveInventoryComponentQuestDto>();
    }
}
