using FluentValidation;
using MediatR;
using ResourceTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest
{
    public class CreateQuestValidator : AbstractValidator<CreateQuestCommand>
    {
        public CreateQuestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(30).WithMessage("Name cannot exceed 30 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
            RuleFor(x => x.Location)
               .MaximumLength(225).WithMessage("Location cannot exceed 225 characters");
        }

    }

}
