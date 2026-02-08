

using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponents
{
    public class DeleteBuildPlanComponentsValidator  : AbstractValidator<DeleteBuildPlanComponentsCommand>
    {
        public DeleteBuildPlanComponentsValidator() 
        {
            RuleFor(x => x.Ids)
                 .NotEmpty().WithMessage("Atleast 1 id is required");

            RuleForEach(x => x.Ids)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
