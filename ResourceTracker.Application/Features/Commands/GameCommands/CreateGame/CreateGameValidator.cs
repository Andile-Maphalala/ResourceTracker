using FluentValidation;


namespace ResourceTracker.Application.Features.Commands.GameCommands.CreateGame
{
    public class CreateGameValidator : AbstractValidator<CreateGameCommand>
    {
        public CreateGameValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(30).WithMessage("Name cannot exceed 30 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
            RuleFor(x => x.GameId)
               .GreaterThan(0).WithMessage("Game is required");
        }
    }
}
