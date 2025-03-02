using FluentValidation;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponents
{
    public class UpdateQuestComponentsValidator : AbstractValidator<UpdateQuestComponentsCommand>
    {
        public UpdateQuestComponentsValidator()
        {
            RuleFor(x => x.Commands)
               .NotEmpty().WithMessage("Atleast 1 command is required");

            RuleForEach(x => x.Commands).SetValidator(new UpdateQuestComponentValidator());
        }
    }
}
