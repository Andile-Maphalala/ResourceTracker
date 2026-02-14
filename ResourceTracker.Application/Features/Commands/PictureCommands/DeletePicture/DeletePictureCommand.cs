
using ResourceTracker.Application.Common.CQRS;

namespace ResourceTracker.Application.Features.Commands.PictureCommands.DeletePicture
{
    public record DeletePictureCommand(int Id) : ICommand;
}
