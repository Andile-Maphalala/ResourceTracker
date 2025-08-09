using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest
{
    public class DeleteQuestValidator : AbstractValidator<DeleteQuestCommand>
    {
        public DeleteQuestValidator() 
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
