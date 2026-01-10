using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Repositories;
using ResourceTracker.ImageStorageService.Models;
using ResourceTracker.ImageStorageService.Services;

namespace ResourceTracker.ImageStorageService
{
    public static class ImageStorageServicesRegistration
    {
        public static IServiceCollection ConfigureImageStorageServices(this IServiceCollection services, string basePath)
        {
            services.AddHttpClient<IImageService, ImageService>();
            services.Configure<ImageStorageOptions>(options =>
            {
                options.BasePath = basePath;
            });
            return services;
        }
    }
}
