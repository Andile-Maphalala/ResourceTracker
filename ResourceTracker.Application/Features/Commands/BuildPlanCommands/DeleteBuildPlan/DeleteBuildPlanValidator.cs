using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanCommands.DeleteBuildPlan
{
    public class DeleteBuildPlanValidator : AbstractValidator<DeleteBuildPlanCommand>
    {
        public DeleteBuildPlanValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
