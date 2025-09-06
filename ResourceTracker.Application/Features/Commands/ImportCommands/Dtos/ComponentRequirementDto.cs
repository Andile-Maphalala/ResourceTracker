
using System.Text.Json.Serialization;

namespace ResourceTracker.Application.Features.Commands.ImportCommands.Dtos
{
    public class ComponentRequirementDto
    {
        [JsonPropertyName("component")]
        public string Compononent { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
