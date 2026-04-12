using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Interfaces;
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
        public async Task<Picture> UploadImage(IFormFile file, int? linkedentityType, string folder, int? uploadedBy, string AltText, CancellationToken cancellationToken)
        {
            var imageStream = await ConvertIFormFileToByteArray(file);
            return await SavePicture(imageStream, linkedentityType, file.FileName, folder, file.ContentType, AltText, uploadedBy, cancellationToken);
        }

        private async Task<Picture>  SavePicture(byte[] imageStream, int? linkedentityType, string fileName, string folder, string contentType, string altText, int? uploadedBy, CancellationToken cancellationToken)
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
                await targetStream.WriteAsync(imageStream,cancellationToken);
            }

            var picture = new Picture
            {
                Path = $"{fullPath.Replace('\\', '/')}",
                Name = fileName,
                ContentType = contentType,
                Size = imageStream.Length,
                CreatedDate = DateTime.UtcNow,
                UploadedBy = uploadedBy,
                AltText = altText,
                LinkedEntityType = linkedentityType
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
                throw new NotFoundException(nameof(Picture), pictureId);
            }

            var storedPath = (picture.Path ?? string.Empty).Trim();
            storedPath = storedPath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            string fullPath;
            if (Path.IsPathRooted(storedPath))
            {
                fullPath = storedPath;
            }
            else
            {
                fullPath = Path.Combine(_baseStoragePath, storedPath);
            }

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (IOException ex)
                {
                    throw new Exception($"Failed to delete image file at '{fullPath}'", ex);
                }
            }

            await _repo.DeleteAsync(picture, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
        }

        public async Task<Picture> UploadImage(string url, int? linkedentityType, string folder, int? uploadedBy, string AltText, CancellationToken cancellationToken)
        {

            byte[] bytes;
            string fileName;
            string contentType;

            if (File.Exists(url))
            {
                bytes = await File.ReadAllBytesAsync(url, cancellationToken);
                fileName = Path.GetFileName(url);
                contentType = GetContentTypeFromExtension(fileName);
            }
            else
            {
                using var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                fileName = Path.GetFileName(new Uri(url).LocalPath);
                contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            }
                

            return await SavePicture(bytes, linkedentityType, fileName, folder, contentType, AltText, uploadedBy, cancellationToken);

        }

        private string GetContentTypeFromExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch(extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                default:
                    return "application/octet-stream";
            }
        }

        private async Task<byte[]> ConvertIFormFileToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file provided");
            }
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                if (memoryStream.Length == 0)
                {
                    throw new InvalidOperationException("Failed to read file content");
                }

                return memoryStream.ToArray();
            }
        }
    }
}
