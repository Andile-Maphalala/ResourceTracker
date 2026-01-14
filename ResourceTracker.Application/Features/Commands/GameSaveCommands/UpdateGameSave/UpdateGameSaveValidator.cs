using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.GameSaveCommands.UpdateGameSave
{
    public class UpdateGameSaveValidator : AbstractValidator<UpdateGameSaveCommand>
    {
        public UpdateGameSaveValidator()
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
