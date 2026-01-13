using FluentValidation;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest
{
    public class DeleteQuestValidator : AbstractValidator<DeleteQuestCommand>
    {
        public DeleteQuestValidator() 
        {
            RuleFor(x => x.Id).GreaterThan(0)
                .WithMessage("Id is required");
        }
    }
}
