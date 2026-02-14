
namespace ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest
{
    public class GetQuestResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string? ImageUrl { get; set; }
        public int GameSaveId { get; set; }
    }
}
