using FluentValidation;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponent
{
    public class DeleteQuestComponentValidator : AbstractValidator<DeleteQuestComponentCommand>
    {
        public DeleteQuestComponentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");
        }
    }
}
