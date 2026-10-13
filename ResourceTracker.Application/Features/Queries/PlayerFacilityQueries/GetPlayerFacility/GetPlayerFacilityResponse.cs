
namespace ResourceTracker.Application.Features.Queries.PlayerFacilityQueries.GetPlayerFacility
{
    public class GetPlayerFacilityResponse
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int ComponentType { get; set; }
        public string ComponentTypeName { get; set; }
        public string? ComponentImageUrl { get; set; }
        public int QuestId { get; set; }
        public string QuestName { get; set; }
        public string? QuestImageUrl { get; set; }
        public int GameSaveId { get; set; }
    }
}
