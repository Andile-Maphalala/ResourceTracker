
namespace ResourceTracker.Application.Features.Commands.ImportCommands.Dtos
{
    public class ImageDto
    {
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string AltText { get; set; }
    }
}
