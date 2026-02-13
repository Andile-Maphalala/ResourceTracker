
namespace ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests
{
    public class SearchQuestsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int GameSaveId { get; set; }
        public string GameSaveName { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }
    }
}
