

namespace ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSaveList
{
    public class GetGameSaveListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
    }
}
