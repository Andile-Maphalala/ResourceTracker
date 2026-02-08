

using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponents
{
    public class CreateBuildPlanComponentsValidator  : AbstractValidator<CreateBuildPlanComponentsCommand>
    {
        public CreateBuildPlanComponentsValidator() 
        {
            RuleFor(x => x.BuildPlanId)
                .GreaterThan(0).WithMessage("BuildPlan is required");
            RuleForEach(x => x.Commands).ChildRules(commands =>
            {
                commands.RuleFor(x => x.ComponentId)
                    .GreaterThan(0).WithMessage("Component is required");
                commands.RuleFor(x => x.QuantityNeeded)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0");
                commands.RuleFor(x => x.Order)
                    .GreaterThanOrEqualTo(0).WithMessage("Order cannot be negative");
            });
        }
    }
}
