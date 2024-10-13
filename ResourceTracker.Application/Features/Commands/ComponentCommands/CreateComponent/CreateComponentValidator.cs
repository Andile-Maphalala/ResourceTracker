using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Commands.ComponentCommands.CreateComponent
{
    public class CreateComponentValidator : IRequest
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public bool IsDebit { get; set; }
        public int UserID { get; set; }
    }

}
