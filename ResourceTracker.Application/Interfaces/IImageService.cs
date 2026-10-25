using Microsoft.AspNetCore.Http;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Interfaces
{
    public interface IImageService
    {
        Task<Picture> UploadImage(IFormFile file, int? linkedentityType, string folder, int? uploadedBy, string AltText, CancellationToken cancellationToken);
        Task<Picture> UploadImage(string url, int? linkedentityType, string folder, int? uploadedBy, string AltText, CancellationToken cancellationToken);
        Task DeleteImage(int pictureId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes image files by their stored paths, ignoring ones that are missing. Used when the
        /// Picture rows have already been removed, so only the files on disk are left to clear.
        /// Returns how many files were actually deleted.
        /// </summary>
        Task<int> DeleteImageFiles(IEnumerable<string?> paths, CancellationToken cancellationToken);
    }
}
