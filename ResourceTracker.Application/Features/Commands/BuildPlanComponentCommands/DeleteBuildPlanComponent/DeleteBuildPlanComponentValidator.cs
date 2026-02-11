

using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponent
{
    public class DeleteBuildPlanComponentValidator  : AbstractValidator<DeleteBuildPlanComponentCommand>
    {
        public DeleteBuildPlanComponentValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
