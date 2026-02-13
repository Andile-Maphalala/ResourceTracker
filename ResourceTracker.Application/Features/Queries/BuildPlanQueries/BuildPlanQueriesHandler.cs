using MediatR;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Features.Queries.BuildPlanQueries.GetBuildPlan;
using ResourceTracker.Application.Features.Queries.BuildPlanQueries.SearchBuildPlans;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Queries.BuildPlanQueries
{
    public class BuildPlanQueriesHandler :
        IRequestHandler<GetBuildPlanQuery, GetBuildPlanResponse>,
        IRequestHandler<SearchBuildPlansQuery, PageableResponse<SearchBuildPlansResponse>>
    {
        private readonly IResourceTrackerRepository _repo;

        public BuildPlanQueriesHandler(IResourceTrackerRepository repo)
        {
            _repo = repo;
        }

        public async Task<GetBuildPlanResponse> Handle(GetBuildPlanQuery request, CancellationToken cancellationToken)
        {
            var plan = await _repo.BuildPlans
                .Where(x => x.Id == request.Id)
                .Select(x => new GetBuildPlanResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    GameSaveId = x.GameSaveId,
                    GameSaveName = x.GameSave.Name
                }).FirstOrDefaultAsync(cancellationToken);

            if (plan is null)
                throw new NotFoundException(nameof(BuildPlan), request.Id);

            return plan;
        }

        public async Task<PageableResponse<SearchBuildPlansResponse>> Handle(SearchBuildPlansQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = nameof(BuildPlan.Id);
            }

            var plans = await _repo.Search(request)
                .Select(x => new SearchBuildPlansResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    GameId = x.GameSave.GameId,
                    GameName = x.GameSave.Game.Name,
                    GameSaveId = x.GameSave.Id,
                    GameSaveName = x.GameSave.Name
                }).ToPageableListAsync(request, cancellationToken);

            return plans;
        }
    }
}
