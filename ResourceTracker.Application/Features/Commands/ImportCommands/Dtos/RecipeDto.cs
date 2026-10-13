


using System.Text.Json.Serialization;

namespace ResourceTracker.Application.Features.Commands.ImportCommands.Dtos
{
    public class RecipeDto
    {
        [JsonPropertyName("output_quantity")]
        public int OutputQuantity { get; set; }
        [JsonPropertyName("station")]
        public string Station { get; set; }
        // Minimum level of that station. Omitted by data that has no levels (Subnautica), where
        // the default of 1 is right: any owned facility satisfies it. 0 appears in the Valheim
        // data and means "any level", not "level zero".
        [JsonPropertyName("station_level")]
        public int StationLevel { get; set; } = 1;
        [JsonPropertyName("requirements")]
        public List<ComponentRequirementDto> Requirements { get; set; }
    }
}
