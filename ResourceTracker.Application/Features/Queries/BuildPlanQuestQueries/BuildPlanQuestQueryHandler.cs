

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.BuildPlanQuestQueries.GetBuildPlanQuestList;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;

namespace ResourceTracker.Application.Features.Queries.BuildPlanQuestQueries
{
    public class BuildPlanQuestQueryHandler : IRequestHandler<GetBuildPlanQuestListQuery, List<GetBuildPlanQuestListResponse>>
    {
        private readonly IResourceTrackerRepository _repo;
        public BuildPlanQuestQueryHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
        }
        public async Task<List<GetBuildPlanQuestListResponse>> Handle(GetBuildPlanQuestListQuery request, CancellationToken cancellationToken)
        {
            if(request == null) 
                return new List<GetBuildPlanQuestListResponse>();

            if(request.BuildPlanId.IsNullOrEmpty() && request.QuestId.IsNullOrEmpty())
                return new List<GetBuildPlanQuestListResponse>();

            var query = _repo.BuildPlanQuests.Include(bpq => bpq.BuildPlan).Include(bpq => bpq.Quest).AsQueryable();

            if(!request.BuildPlanId.IsNullOrEmpty())
            {
                query = query.Where(bpq => bpq.BuildPlanId == request.BuildPlanId.Value);
            }

            if(!request.QuestId.IsNullOrEmpty())
            {
                query = query.Where(bpq => bpq.QuestId == request.QuestId.Value);
            }

            var response = await query
            .Select(bpq => new GetBuildPlanQuestListResponse
                {
                    BuildPlanId = bpq.BuildPlanId,
                    BuildPlanName = bpq.BuildPlan.Name,
                    BuildPlanDescription = bpq.BuildPlan.Description,
                    QuestId = bpq.QuestId,
                    QuestName = bpq.Quest.Name,
                    QuestDescription = bpq.Quest.Description
                }).ToListAsync(cancellationToken);
            return response;
        }
    }
}
