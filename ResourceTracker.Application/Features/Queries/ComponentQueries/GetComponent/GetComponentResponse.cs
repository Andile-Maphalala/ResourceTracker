
namespace ResourceTracker.Application.Features.Queries.ComponentQueries.GetComponent
{
    public class GetComponentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
