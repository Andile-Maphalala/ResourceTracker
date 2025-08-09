using FluentValidation;
using MediatR;
using ResourceTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.UpdateQuest
{
    public class UpdateQuestValidator : AbstractValidator<UpdateQuestCommand>
    {
        public UpdateQuestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
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

