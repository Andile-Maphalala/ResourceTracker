
using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponent
{
    public class UpdateBuildPlanComponentValidator  : AbstractValidator<UpdateBuildPlanComponentCommand>
    {
        public UpdateBuildPlanComponentValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
            RuleFor(x => x.QuantityNeeded)
                 .GreaterThan(0).WithMessage("Quantity must be greater than 0");
            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Order cannot be negative");
        }
    }
}
