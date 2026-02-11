using MediatR;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;


namespace ResourceTracker.Application.Features.Queries.QuestQueries
{
    public class QuestQueriesHandler:
        IRequestHandler<GetQuestQuery,GetQuestResponse>,
        IRequestHandler<SearchQuestsQuery, PageableResponse<SearchQuestsResponse>>
    {
        private readonly IResourceTrackerRepository _repo;

        public QuestQueriesHandler(IResourceTrackerRepository repo)
        {
            _repo = repo;
        }

        public async Task<GetQuestResponse> Handle(GetQuestQuery request, CancellationToken cancellationToken)
        {
            var quest = await _repo.Quests
                 .Where(x => x.Id == request.Id)
                 .Select(x => new GetQuestResponse
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     Location = x.Location,
                     GameSaveId = x.GameSaveId

                 }).FirstOrDefaultAsync(cancellationToken);

            if (quest is null)
                throw new NotFoundException(nameof(Quest), request.Id);

            return quest;
        }

        public async Task<PageableResponse<SearchQuestsResponse>> Handle(SearchQuestsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = nameof(Quest.Id);
            }

            var quests = await _repo.Search(request)
                 .Select(x => new SearchQuestsResponse
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     Location = x.Location,
                     GameId = x.GameSave.GameId,
                     GameName = x.GameSave.Game.Name,
                     GameSaveId = x.GameSave.Id,
                     GameSaveName = x.GameSave.Name
                 }).ToPageableListAsync(request, cancellationToken);

            return quests;
        }
    }
}
