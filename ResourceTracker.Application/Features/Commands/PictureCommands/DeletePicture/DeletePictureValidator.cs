using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.PictureCommands.DeletePicture
{
    public class DeletePictureValidator : AbstractValidator<DeletePictureCommand>
    {
        public DeletePictureValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Picture ID is required.");
        }
    }
}
