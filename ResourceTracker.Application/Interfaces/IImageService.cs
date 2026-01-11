using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Interfaces
{
    public interface IImageService
    {
        Task<Picture> UploadImage(byte[] imageStream, string folder, string fileName, string contentType, int? uploadedBy, string AltText, CancellationToken cancellationToken);
        Task<Picture> UploadImage(string url, string folder, int? uploadedBy, string AltText, CancellationToken cancellationToken);
        Task DeleteImage(int pictureId, CancellationToken cancellationToken);
        Task<Picture> GetImage(int pictureId, CancellationToken cancellationToken);
    }
}
