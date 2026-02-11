
using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponents
{
    public class UpdateBuildPlanComponentsValidator  : AbstractValidator<UpdateBuildPlanComponentsCommand>
    {
        public UpdateBuildPlanComponentsValidator() 
        {
            RuleFor(x => x.Commands).NotEmpty().WithMessage("Atleast 1 command is required");
            RuleForEach(x => x.Commands).ChildRules(commands =>
            {
                commands.RuleFor(x => x.Id)
                    .GreaterThan(0).WithMessage("Id is required");
                commands.RuleFor(x => x.QuantityNeeded)
                     .GreaterThan(0).WithMessage("Quantity must be greater than 0");
                commands.RuleFor(x => x.Order)
                    .GreaterThanOrEqualTo(0).WithMessage("Order cannot be negative");
            });
        }
    }
}
