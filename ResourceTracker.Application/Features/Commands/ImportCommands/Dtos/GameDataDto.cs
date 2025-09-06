

using System.Text.Json.Serialization;

namespace ResourceTracker.Application.Features.Commands.ImportCommands.Dtos
{
    public class GameDataDto
    {
        [JsonPropertyName("game")]
        public string Name { get; set; }
        [JsonPropertyName("components")]
        public List<GameComponentDto> Components { get; set; }
    }
}
