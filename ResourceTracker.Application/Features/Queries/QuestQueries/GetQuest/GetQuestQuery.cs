using MediatR;


namespace ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest
{
    public class GetQuestQuery : IRequest<GetQuestResponse>
    {
        public int Id { get; set; }
    }
}
