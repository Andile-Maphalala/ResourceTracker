using MediatR;

namespace ResourceTracker.Application.Features.Queries.QuestComponentQueres.GetQuestComponent
{
    public record GetQuestComponentQuery(int Id) : IRequest<GetQuestComponentResponse>;
}
