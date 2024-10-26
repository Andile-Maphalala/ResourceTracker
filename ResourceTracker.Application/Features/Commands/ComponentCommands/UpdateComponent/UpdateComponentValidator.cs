using FluentValidation;
using MediatR;
using ResourceTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent
{
    public class UpdateComponentValidator : AbstractValidator<UpdateComponentCommand>
    {
        public UpdateComponentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.Name)
                .MaximumLength(30).WithMessage("Name cannot exceed 30 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
            RuleFor(x => x.Type)
            .NotNull().WithMessage("Type is required")
             .Must(IsValidEnumValue).WithMessage("Type is not a valid component type."); ;
        }
        private bool IsValidEnumValue(int type)
        {
            return Enum.IsDefined(typeof(ComponentTypeEnum), type);
        }
    }
    }

