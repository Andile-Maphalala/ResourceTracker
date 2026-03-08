

namespace ResourceTracker.Application.Features.Queries.PictureQueries.GetPictureList
{
    public class GetPictureListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AltText { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public int? ImageUploadType { get; set; }
        public string ImageUploadTypeName { get; set; }
    }
}
