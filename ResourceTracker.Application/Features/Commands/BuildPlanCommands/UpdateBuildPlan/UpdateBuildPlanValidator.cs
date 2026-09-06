using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanCommands.UpdateBuildPlan
{
    public class UpdateBuildPlanValidator : AbstractValidator<UpdateBuildPlanCommand>
    {
        public UpdateBuildPlanValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");

            RuleFor(x => x.IncludeInventory)
                .NotNull().WithMessage("IncludeInventory is required");

            RuleFor(x => x.IncludeFacilityRequirements)
                .NotNull().WithMessage("IncludeFacilityRequirements is required");
        }
    }
}
