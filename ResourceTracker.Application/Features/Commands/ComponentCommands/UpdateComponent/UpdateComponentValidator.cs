using FluentValidation;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent
{
    public class UpdateComponentValidator : AbstractValidator<UpdateComponentCommand>
    {
        public UpdateComponentValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
            RuleFor(x => x.Type)
            .GreaterThan(0).WithMessage("Type is required")
             .Must(IsValidEnumValue).WithMessage("Type is not a valid component type");
        }
        private bool IsValidEnumValue(int type)
        {
            return Enum.IsDefined(typeof(ComponentTypeEnum), type);
        }
    }
}

