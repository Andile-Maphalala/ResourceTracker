using MediatR;


namespace ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest
{
    public record GetQuestQuery(int Id) : IRequest<GetQuestResponse>;
}
