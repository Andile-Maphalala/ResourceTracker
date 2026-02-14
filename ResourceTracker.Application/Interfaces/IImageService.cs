using Microsoft.AspNetCore.Http;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Interfaces
{
    public interface IImageService
    {
        Task<Picture> UploadImage(IFormFile file, string folder, int? uploadedBy, string AltText, CancellationToken cancellationToken);
        Task<Picture> UploadImage(string url, string folder, int? uploadedBy, string AltText, CancellationToken cancellationToken);
        Task DeleteImage(int pictureId, CancellationToken cancellationToken);
    }
}
