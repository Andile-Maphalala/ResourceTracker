using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponent
{
    public class DeleteQuestComponentValidator : AbstractValidator<DeleteQuestComponentCommand>
    {
        public DeleteQuestComponentValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
        }
    }
}
