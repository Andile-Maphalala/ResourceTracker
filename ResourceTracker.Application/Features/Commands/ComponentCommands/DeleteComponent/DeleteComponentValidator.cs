using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent
{
    public class DeleteComponentValidator : AbstractValidator<DeleteComponentCommand>
    {
        public DeleteComponentValidator() 
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
