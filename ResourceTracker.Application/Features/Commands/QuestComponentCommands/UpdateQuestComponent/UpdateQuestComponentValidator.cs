using FluentValidation;
using ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent
{
    public class UpdateQuestComponentValidator : AbstractValidator<UpdateQuestComponentCommand>
    {
        public UpdateQuestComponentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.AmountAquired)
                .GreaterThanOrEqualTo(0).WithMessage("Amount aquired cannot be less than 1");
        }
    }
}
