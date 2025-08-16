

namespace ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSave
{
    public class GetGameSaveResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }
    }
}
