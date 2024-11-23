using MediatR;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.ComponentQueries.GetComponent;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;
using ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;
using ResourceTracker.Application.QueryBuilders;
using ResourceTracker.Application.Repositories;
using ResourceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                     Description = x.Description
                 }).FirstOrDefaultAsync(cancellationToken);

            if (quest is null)
                throw new NotFoundException(nameof(Quest), request.Id);

            return quest;
        }

        public async Task<PageableResponse<SearchQuestsResponse>> Handle(SearchQuestsQuery request, CancellationToken cancellationToken)
        {
            var quests = await _repo.Quests
                   .ApplyFilters(request)
                 .Select(x => new SearchQuestsResponse
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description
                 }).ToPageableListAsync(request,cancellationToken);

            return quests;
        }
    }
}
