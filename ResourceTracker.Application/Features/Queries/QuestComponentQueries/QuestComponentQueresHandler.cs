using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pagination;
using Pagination.Models;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.GetQuestComponent;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;



namespace ResourceTracker.Application.Features.Queries.QuestComponentQueres
{
    public class QuestComponentQueresHandler : IRequestHandler<GetQuestComponentQuery,GetQuestComponentResponse>,
                                               IRequestHandler<SearchQuestComponentsQuery, PageableResponse<SearchQuestComponentsResponse>>
    {

        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;

        public QuestComponentQueresHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;
        }

        public async Task<GetQuestComponentResponse> Handle(GetQuestComponentQuery request, CancellationToken cancellationToken)
        {
            var response = await _repo.QuestComponents
                 .Where(x => x.Id == request.Id)
                 .Select(x => new GetQuestComponentResponse
                 {
                    Id = x.Id,
                    AmountAquired = x.AmountAquired,
                    ComponentId = x.ComponentId,
                    ComponentName = x.Component.Name,
                    ComponentType = x.Component.Type,
                    ComponentTypeName = EnumHelper.GetEnumDescription((ComponentTypeEnum)x.Component.Type),
                    ComponentImageUrl = x.Component.Picture.GetFileUrl(_baseUrl),
                    QuestId = x.QuestId,
                    QuestName = x.Quest.Name,
                    QuestImageUrl = x.Quest.Picture.GetFileUrl(_baseUrl)

                 }).FirstOrDefaultAsync(cancellationToken);

            if (response is null)
                throw new NotFoundException("Quest component", request.Id);

            return response;
        }

        public async Task<PageableResponse<SearchQuestComponentsResponse>> Handle(SearchQuestComponentsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.OrderBy))
                request.OrderBy = nameof(QuestComponents.Id);

            var response = await _repo.Search(request)
                   .Select(x => new SearchQuestComponentsResponse
                   {
                       Id = x.Id,
                       AmountAquired = x.AmountAquired,
                       ComponentId = x.ComponentId,
                       ComponentName = x.Component.Name,
                       ComponentType = x.Component.Type,
                       ComponentTypeName = EnumHelper.GetEnumDescription((ComponentTypeEnum)x.Component.Type),
                       ComponentImageUrl = x.Component.Picture.GetFileUrl(_baseUrl),
                       QuestId = x.QuestId,
                       QuestName = x.Quest.Name,
                       QuestImageUrl = x.Quest.Picture.GetFileUrl(_baseUrl)
                   }).ToPageableListAsync(request, cancellationToken);

            return response;
        }
    }
}
