using FluentValidation;
using ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponents
{
    public class CreateQuestComponentsValidator : AbstractValidator<CreateQuestComponentsCommand>
    {
        public CreateQuestComponentsValidator()
        {

            RuleFor(x => x.QuestId)
               .NotEmpty().WithMessage("Quest is required");

            RuleForEach(x => x.Commands).ChildRules(commands =>
            {
                commands.RuleFor(x => x.AmountRequired)
               .GreaterThan(0).WithMessage("Amount required must at least be 1");
                commands.RuleFor(x => x.AmountAquired)
                    .GreaterThanOrEqualTo(0).WithMessage("Amount aquired cannot be less than 1");
                commands.RuleFor(x => x.ComponentId)
                    .NotEmpty().WithMessage("Component is required");
            });
        }
    }
}
