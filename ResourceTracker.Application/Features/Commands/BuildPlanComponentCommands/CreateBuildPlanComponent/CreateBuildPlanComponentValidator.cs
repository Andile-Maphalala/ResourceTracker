
using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponent
{
    public class CreateBuildPlanComponentValidator  : AbstractValidator<CreateBuildPlanComponentCommand> 
    {
        public CreateBuildPlanComponentValidator()
        {
            RuleFor(x => x.BuildPlanId).
                GreaterThan(0).WithMessage("BuildPlan is required");
            RuleFor(x => x.ComponentId)
                .GreaterThan(0).WithMessage("Component is required");
            RuleFor(x => x.QuantityNeeded)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");
            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Order cannot be negative");
        }
    }
}
