using Microsoft.AspNetCore.Http;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Models.Enums;

namespace ResourceTracker.Application.Features.Commands.PictureCommands.CreatePicture
{
    public class CreatePictureCommand : ICommand<CreatePictureResponse>
    {
        public string Name { get; set; }
        public string? AltText { get; set; }
        public IFormFile Data { get; set; }
        public ImageUploadTypeEnum ImageUploadType { get; set; }
        public int LinkedEntityId { get; set; }
    }
}
