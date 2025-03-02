using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Queries.QuestComponentQueres.GetQuestComponent
{
    public class GetQuestComponentQuery : IRequest<GetQuestComponentResponse>
    {
        public int Id { get; set; }
    }
}
