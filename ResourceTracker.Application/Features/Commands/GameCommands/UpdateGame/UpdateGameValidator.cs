using FluentValidation;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame
{
    public class UpdateGameValidator : AbstractValidator<UpdateGameCommand>
    {
        public UpdateGameValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required");
            RuleFor(x => x.Name)
                .MaximumLength(30).WithMessage("Name cannot exceed 30 characters")
                .NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Description)
                .MaximumLength(225).WithMessage("Description cannot exceed 225 characters");
            RuleFor(x => x.GameId)
               .GreaterThan(0).WithMessage("Game is required");
        }
    }
}
