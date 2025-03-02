using FluentValidation;
using ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent
{
    public class CreateQuestComponentValidator : AbstractValidator<CreateQuestComponentCommand>
    {
        public CreateQuestComponentValidator()
        {
            RuleFor(x => x.AmountRequired)
                .GreaterThan(0).WithMessage("Amount required must at least be 1");
            RuleFor(x => x.AmountAquired)
                .GreaterThanOrEqualTo(0).WithMessage("Amount aquired cannot be less than 1");
            RuleFor(x => x.ComponentId)
                .NotEmpty().WithMessage("Component is required");
            RuleFor(x => x.QuestId)
               .NotEmpty().WithMessage("Quest is required");
        }
    }
}
