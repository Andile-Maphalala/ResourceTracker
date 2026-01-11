using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame
{
    public class UpdateGameValidator : AbstractValidator<UpdateGameCommand>
    {
        public UpdateGameValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
        }
    }
}
