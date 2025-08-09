using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Repositories;
using ResourceTracker.ImageStorageService.Services;

namespace ResourceTracker.ImageStorageService
{
    public static class ImageStorageServicesRegistration
    {
        public static IServiceCollection ConfigureImageStorageServices(this IServiceCollection services)
        {
            services.AddScoped<IImageService, ImageService>();

            return services;
        }
    }
}
