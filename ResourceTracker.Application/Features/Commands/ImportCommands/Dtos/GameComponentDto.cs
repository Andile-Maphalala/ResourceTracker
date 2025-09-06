

using System.Text.Json.Serialization;

namespace ResourceTracker.Application.Features.Commands.ImportCommands.Dtos
{
    public class GameComponentDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("image_url")]
        public string Image_url { get; set; }
        [JsonPropertyName("rawMaterial")]
        public bool RawMaterial { get; set; }
        [JsonPropertyName("recipe")]
        public RecipeDto? Recipe { get; set; }
    }
}
