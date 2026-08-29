

namespace ResourceTracker.Application.Features.Queries.InventoryQueries.GetInventoryComponentQuest
{
    public class GetInventoryComponentQuestResponse
    {
        public int QuestComponentId { get; set; }
        public int QuestId { get; set; }
        public string QuestName { get; set; }
        public int Quantity { get; set; }
    }
}
