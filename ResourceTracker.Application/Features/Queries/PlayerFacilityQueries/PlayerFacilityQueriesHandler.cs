using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.PlayerFacilityQueries.GetPlayerFacility;
using ResourceTracker.Application.Features.Queries.PlayerFacilityQueries.GetPlayerFacilitiesByQuest;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Queries.PlayerFacilityQueries
{
    public class PlayerFacilityQueriesHandler : IRequestHandler<GetPlayerFacilityQuery, GetPlayerFacilityResponse>,
                                                IRequestHandler<GetPlayerFacilitiesByQuestQuery, List<GetPlayerFacilitiesByQuestResponse>>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;

        public PlayerFacilityQueriesHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;
        }

        public async Task<GetPlayerFacilityResponse> Handle(GetPlayerFacilityQuery request, CancellationToken cancellationToken)
        {
            var response = await _repo.PlayerFacilities
                 .Where(x => x.Id == request.Id)
                 .Select(x => new GetPlayerFacilityResponse
                 {
                     Id = x.Id,
                     Level = x.Level,
                     ComponentId = x.ComponentId,
                     ComponentName = x.Component.Name,
                     ComponentType = x.Component.Type,
                     ComponentTypeName = EnumHelper.GetEnumDescription((ComponentTypeEnum)x.Component.Type),
                     ComponentImageUrl = x.Component.Picture.GetFileUrl(_baseUrl),
                     QuestId = x.QuestId,
                     QuestName = x.Quest.Name,
                     QuestImageUrl = x.Quest.Picture.GetFileUrl(_baseUrl),
                     GameSaveId = x.GameSaveId

                 }).FirstOrDefaultAsync(cancellationToken);

            if (response is null)
                throw new NotFoundException("Player facility", request.Id);

            return response;
        }

        public async Task<List<GetPlayerFacilitiesByQuestResponse>> Handle(GetPlayerFacilitiesByQuestQuery request, CancellationToken cancellationToken)
        {
            var questExists = await _repo.Quests.AnyAsync(x => x.Id == request.QuestId, cancellationToken);
            if (!questExists)
                throw new NotFoundException(nameof(Quest), request.QuestId);

            return await _repo.PlayerFacilities
                 .Where(x => x.QuestId == request.QuestId)
                 .OrderBy(x => x.Component.Name)
                 .Select(x => new GetPlayerFacilitiesByQuestResponse
                 {
                     Id = x.Id,
                     Level = x.Level,
                     ComponentId = x.ComponentId,
                     ComponentName = x.Component.Name,
                     ComponentType = x.Component.Type,
                     ComponentTypeName = EnumHelper.GetEnumDescription((ComponentTypeEnum)x.Component.Type),
                     ComponentImageUrl = x.Component.Picture.GetFileUrl(_baseUrl),
                     QuestId = x.QuestId,
                     QuestName = x.Quest.Name,
                     QuestImageUrl = x.Quest.Picture.GetFileUrl(_baseUrl),
                     GameSaveId = x.GameSaveId
                 }).ToListAsync(cancellationToken);
        }
    }
}
