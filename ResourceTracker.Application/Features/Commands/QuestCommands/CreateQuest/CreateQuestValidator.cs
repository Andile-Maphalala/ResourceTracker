using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest
{
    public class CreateQuestValidator : AbstractValidator<CreateQuestCommand>
    {
        public CreateQuestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
            RuleFor(x => x.Location)
               .MaximumLength(225).WithMessage("Location cannot exceed 225 characters");
            RuleFor(x => x.GameSaveId)
                .GreaterThan(0).WithMessage("Game Save is required");
        }

    }

}
