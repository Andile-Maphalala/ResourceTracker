using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;


namespace ResourceTracker.Application.Features.Queries.QuestQueries
{
    public class QuestQueriesHandler:
        IRequestHandler<GetQuestQuery,GetQuestResponse>,
        IRequestHandler<SearchQuestsQuery, PageableResponse<SearchQuestsResponse>>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;
        public QuestQueriesHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;
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
                     GameSaveId = x.GameSaveId,
                     ImageUrl = x.Picture.GetFileUrl(_baseUrl)

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
                     ImageUrl = x.Picture.GetFileUrl(_baseUrl),
                     GameId = x.GameSave.GameId,
                     GameName = x.GameSave.Game.Name,
                     GameSaveId = x.GameSave.Id,
                     GameSaveName = x.GameSave.Name
                 }).ToPageableListAsync(request, cancellationToken);

            return quests;
        }
    }
}
