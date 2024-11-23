using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest
{
    public class GetQuestQuery : IRequest<GetQuestResponse>
    {
        public int Id { get; set; }
    }
}
