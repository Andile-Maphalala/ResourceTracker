using Microsoft.AspNetCore.Http;
using System.Text.Json;


public static class FormFileExtensions
{
    public static async Task<T> ToObjectAsync<T>(this IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return await JsonSerializer.DeserializeAsync<T>(stream)
               ?? throw new InvalidOperationException("Failed to deserialize file to object.");
    }
}
