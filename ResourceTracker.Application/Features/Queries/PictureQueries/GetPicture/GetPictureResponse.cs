

namespace ResourceTracker.Application.Features.Queries.PictureQueries.GetPicture
{
    public class GetPictureResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AltText { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
    }
}
