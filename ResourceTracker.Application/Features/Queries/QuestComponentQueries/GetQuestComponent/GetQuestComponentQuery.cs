using MediatR;

namespace ResourceTracker.Application.Features.Queries.QuestComponentQueres.GetQuestComponent
{
    public class GetQuestComponentQuery : IRequest<GetQuestComponentResponse>
    {
        public int Id { get; set; }
    }
}
