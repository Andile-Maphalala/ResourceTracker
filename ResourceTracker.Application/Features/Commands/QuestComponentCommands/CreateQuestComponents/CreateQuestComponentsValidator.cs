using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponents
{
    public class CreateQuestComponentsValidator : AbstractValidator<CreateQuestComponentsCommand>
    {
        public CreateQuestComponentsValidator()
        {

            RuleFor(x => x.QuestId)
               .GreaterThan(0).WithMessage("Quest is required");
            RuleForEach(x => x.Commands).ChildRules(commands =>
            {
                commands.RuleFor(x => x.AmountAquired)
                    .GreaterThanOrEqualTo(0).WithMessage("Amount aquired cannot be less than 1");
                commands.RuleFor(x => x.ComponentId)
                    .GreaterThan(0).WithMessage("Component is required");
            });
        }
    }
}
