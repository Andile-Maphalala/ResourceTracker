


using System.Text.Json.Serialization;

namespace ResourceTracker.Application.Features.Commands.ImportCommands.Dtos
{
    public class RecipeDto
    {
        [JsonPropertyName("output_quantity")]
        public int OutputQuantity { get; set; }
        [JsonPropertyName("station")]
        public string Station { get; set; }
        [JsonPropertyName("requirements")]
        public List<ComponentRequirementDto> Requirements { get; set; }
    }
}
