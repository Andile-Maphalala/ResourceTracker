using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.DeleteBuildPlanQuest
{
    public class DeleteBuildPlanQuestValidator : AbstractValidator<DeleteBuildPlanQuestCommand>
    {
        public DeleteBuildPlanQuestValidator()
        {
            RuleFor(x => x.BuildPlanId)
                .GreaterThan(0).WithMessage("BuildPlanId is required");
            RuleFor(x => x.QuestId)
                .GreaterThan(0).WithMessage("QuestId is required");
        }
    }
}
