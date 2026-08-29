


namespace ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventorySummary
{
    public class GetGameSaveInventorySummaryResponse
    {
        public int Quests { get; set; }
        public int UniqueItems { get; set; }
        public int TotalItems { get; set; }
        public int ItemStacks { get; set; }
    }
}
