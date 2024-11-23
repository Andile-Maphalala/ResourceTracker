using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest
{
    public class DeleteQuestCommand : IRequest
    {
        public int Id { get; set; }
    }
}
