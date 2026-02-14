
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Common.Helper
{
    public static class ImageHelper
    {
        public const int MaxFileSizeInBytes = 5 * 1024 * 1024; // 5MB

        public static readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".gif"
        };

        public static string GetFileUrl(this Picture picture, string baseUrl)
        {
            if (picture == null)
            {
                return string.Empty;
            }

            return GetFileUrl(picture.Path, baseUrl);
        }

        public static string GetFileUrl(string Path,string baseUrl)
        {
            if (Path == null)
            {
                return string.Empty;
            }

            // Normalize separators
            var path = (Path ?? string.Empty).Replace('\\', '/').Trim();

            if (string.IsNullOrEmpty(path))
                return baseUrl?.TrimEnd('/') ?? string.Empty;

            // If the stored path is already an absolute URL, return it unchanged
            if (Uri.TryCreate(path, UriKind.Absolute, out var maybeUri) &&
                (maybeUri.Scheme == Uri.UriSchemeHttp || maybeUri.Scheme == Uri.UriSchemeHttps))
            {
                return path;
            }

            // Remove filesystem prefixes like ".../app/wwwroot/" or ".../wwwroot/"
            var lower = path.ToLowerInvariant();
            var key = "app/wwwroot";
            var idx = lower.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                path = path.Substring(idx + key.Length);
            }
            else
            {
                key = "wwwroot";
                idx = lower.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    path = path.Substring(idx + key.Length);
                }
            }

            // Trim any leading slashes so we get exactly one between baseUrl and path
            path = path.TrimStart('/');

            baseUrl = (baseUrl ?? string.Empty).TrimEnd('/');

            return string.IsNullOrEmpty(baseUrl) ? "/" + path : $"{baseUrl}/{path}";
        }
    }
}
