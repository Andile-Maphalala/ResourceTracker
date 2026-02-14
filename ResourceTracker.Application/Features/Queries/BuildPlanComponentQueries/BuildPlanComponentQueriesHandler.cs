
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.GetBuildPlanComponent;
using ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries.SearchBuildPlanComponent;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Application.Features.Queries.BuildPlanComponentQueries
{
    public class BuildPlanComponentQueriesHandler :
        IRequestHandler<SearchBuildPlanComponentQuery, PageableResponse<SearchBuildPlanComponentResponse>>,
        IRequestHandler<GetBuildPlanComponentQuery, GetBuildPlanComponentResponse>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;

        public BuildPlanComponentQueriesHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;
        }

        public async Task<GetBuildPlanComponentResponse> Handle(GetBuildPlanComponentQuery request, CancellationToken cancellationToken)
        {
            var response = await _repo.BuildPlanComponents
                .Where(x => x.Id == request.Id)
                .Select(x => new GetBuildPlanComponentResponse
                {
                    Id = x.Id,
                    Order = x.Order,
                    QuantityNeeded = x.QuantityNeeded,
                    ComponentId = x.ComponentId,
                    ComponentName = x.Component.Name,
                    ComponentType = x.Component.Type,
                    ComponentTypeName = x.Component.Type.ToString(),
                    BuildPlanId = x.BuildPlanId,
                    BuildPlanName = x.BuildPlan.Name,
                    ComponentImageUrl = x.Component.Picture.GetFileUrl(_baseUrl)

                }).FirstOrDefaultAsync(cancellationToken);

            if(response is null)
            {
                throw new NotFoundException(nameof(BuildPlanComponent), request.Id);
            }

            return response;
        }

        public async Task<PageableResponse<SearchBuildPlanComponentResponse>> Handle(SearchBuildPlanComponentQuery request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = nameof(BuildPlanComponent.Id);
            }

            var response = await _repo.Search(request)
                .Select(x => new SearchBuildPlanComponentResponse
                {
                    Id = x.Id,
                    Order = x.Order,
                    QuantityNeeded = x.QuantityNeeded,
                    ComponentId = x.ComponentId,
                    ComponentName = x.Component.Name,
                    ComponentType = x.Component.Type,
                    ComponentTypeName = x.Component.Type.ToString(),
                    BuildPlanId = x.BuildPlanId,
                    BuildPlanName = x.BuildPlan.Name,
                    ComponentImageUrl = x.Component.Picture.GetFileUrl(_baseUrl)
                }).ToPageableListAsync(request, cancellationToken);

            return response;
        }
    }
}
