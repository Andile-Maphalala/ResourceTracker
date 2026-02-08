
using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponents
{
    public class DeleteQuestComponentsValidator : AbstractValidator<DeleteQuestComponentsCommand>
    {
        public DeleteQuestComponentsValidator()
        {
            RuleFor(x => x.Ids)
                .NotEmpty().WithMessage("Atleast 1 id is required");

            RuleForEach(x => x.Ids)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
