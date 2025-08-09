using ResourceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Repositories
{
    public interface IImageService
    {
        Task<Picture> UploadImage(byte[] imageStream, string folder, string fileName, string contentType, int? uploadedBy, string AltText, CancellationToken cancellationToken);
        Task DeleteImage(int pictureId, CancellationToken cancellationToken);
        Task<Picture> GetImage(int pictureId, CancellationToken cancellationToken);
    }
}
