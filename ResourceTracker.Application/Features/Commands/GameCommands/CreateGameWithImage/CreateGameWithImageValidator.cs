using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.GameCommands.CreateGame
{
    public class CreateGameWithImageValidator : AbstractValidator<CreateGameWithImageCommand>
    {
        public CreateGameWithImageValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
        }
    }
}
