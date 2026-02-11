using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.CreateBuildPlanQuest
{
    public class CreateBuildPlanQuestValidator : AbstractValidator<CreateBuildPlanQuestCommand>
    {
        public CreateBuildPlanQuestValidator()
        {
            RuleFor(x => x.BuildPlanId)
                .GreaterThan(0).WithMessage("BuildPlanId is required");
            RuleFor(x => x.QuestId)
                .GreaterThan(0).WithMessage("QuestId is required");
        }
    }
}
