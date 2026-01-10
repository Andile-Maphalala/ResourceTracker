using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Repositories;
using ResourceTracker.Domain.Entities;
using ResourceTracker.ImageStorageService.Models;

namespace ResourceTracker.ImageStorageService.Services
{
    public class ImageService : IImageService
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _baseStoragePath;
        private readonly HttpClient _httpClient;

        public ImageService(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IOptions<ImageStorageOptions> baseStoragePath, HttpClient httpClient)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _baseStoragePath = Path.Combine(baseStoragePath.Value.BasePath, "Images");
            _httpClient = httpClient;
        }
        public async Task<Picture> UploadImage(byte[] imageStream, string folder, string fileName, string contentType, int? uploadedBy, string AltText, CancellationToken cancellationToken)
        {
            var storedName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var folderPath = Path.Combine(_baseStoragePath, folder);
            var fullPath = Path.Combine(folderPath, storedName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
            using (var targetStream = System.IO.File.Create(fullPath))
            {
                await targetStream.WriteAsync(imageStream);
            }

            var picture = new Picture
            {
                Path = $"{fullPath.Replace('\\', '/')}",
                Name = fileName,
                ContentType = contentType,
                Size = imageStream.Length,
                CreatedDate = DateTime.UtcNow,
                UploadedBy = uploadedBy,
                AltText = AltText
            };

            await _repo.InsertAsync(picture, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return picture;
        }

        public async Task DeleteImage(int pictureId, CancellationToken cancellationToken)
        {
            var picture = await _repo.Pictures.FirstOrDefaultAsync(x => x.Id == pictureId,cancellationToken);
            if (picture == null)
            {
                throw new NotFoundException("Picture not found");
            }

            var fullPath = Path.Combine(_baseStoragePath, picture.Path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            await _repo.DeleteAsync(picture, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
        }

        public async Task<Picture> GetImage(int pictureId, CancellationToken cancellationToken)
        {
            var picture = await _repo.Pictures.FirstOrDefaultAsync(x => x.Id == pictureId, cancellationToken);

            return picture;
        }

        public async Task<Picture> UploadImage(string url, string folder, int? uploadedBy, string AltText, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var fileName = Path.GetFileName(new Uri(url).LocalPath);
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

            return await UploadImage(bytes, folder, fileName, contentType, uploadedBy, AltText, cancellationToken);
        }
    }
}
