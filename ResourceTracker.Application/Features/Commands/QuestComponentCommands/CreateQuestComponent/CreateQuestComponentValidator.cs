using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent
{
    public class CreateQuestComponentValidator : AbstractValidator<CreateQuestComponentCommand>
    {
        public CreateQuestComponentValidator()
        {
            RuleFor(x => x.AmountAquired)
                .GreaterThanOrEqualTo(0).WithMessage("Amount aquired cannot be less than 0");
            RuleFor(x => x.ComponentId)
                .GreaterThan(0).WithMessage("Component is required");
            RuleFor(x => x.QuestId)
               .GreaterThan(0).WithMessage("Quest is required");
        }
    }
}
