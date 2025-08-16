using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.GameSaveCommands.CreateGameSave
{
    public class CreateGameSaveValidator : AbstractValidator<CreateGameSaveCommand>
    {
        public CreateGameSaveValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
            RuleFor(x => x.GameId)
                .GreaterThan(0).WithMessage("Invalid game selected");
        }
    }
}
