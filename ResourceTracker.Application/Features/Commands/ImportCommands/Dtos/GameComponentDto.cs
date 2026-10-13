

using System.Text.Json.Serialization;

namespace ResourceTracker.Application.Features.Commands.ImportCommands.Dtos
{
    public class GameComponentDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description
        {
            get => _description;
            set => _description = value ?? string.Empty;
        }
        private string _description = string.Empty;
        [JsonPropertyName("image_url")]
        public string Image_url { get; set; }
        [JsonPropertyName("rawMaterial")]
        public bool RawMaterial { get; set; }
        // Declared, not inferred: "Resource", "Composite" or "Facility", matching
        // ComponentTypeEnum. This replaced working the type out from rawMaterial and station
        // names, which quietly mistyped anything that was not a station. Generators write it;
        // a component without one is rejected rather than guessed at.
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("recipe")]
        public RecipeDto? Recipe { get; set; }
    }
}
