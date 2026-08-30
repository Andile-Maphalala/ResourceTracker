using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventoryComponentSummary.Dto;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventorySummary;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventoryTotals;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetInventoryComponentQuest;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Queries.InventoryQueries
{
    public class InventoryQueries : IRequestHandler<GetInventoryComponentQuestQuery, List<GetInventoryComponentQuestResponse>>,
                                                                    IRequestHandler<GetGameSaveInventoryComponentSummaryQuery, List<GetGameSaveInventoryComponentSummaryResponse>>,
                                                                    IRequestHandler<GetGameSaveInventorySummaryQuery, GetGameSaveInventorySummaryResponse>
    {


        private readonly string _baseUrl;
        private readonly IResourceTrackerRepository _repo;
        public InventoryQueries(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;

        }

        public async Task<List<GetInventoryComponentQuestResponse>> Handle(GetInventoryComponentQuestQuery request, CancellationToken cancellationToken)
        {
            var query = from qc in _repo.QuestComponents.AsNoTracking()
                        join bpq in _repo.BuildPlanQuests on qc.QuestId equals bpq.QuestId
                        where qc.ComponentId == request.ComponentId
                              && bpq.BuildPlanId == request.BuildPlanId
                        select new GetInventoryComponentQuestResponse
                        {
                            QuestComponentId = qc.Id,
                            QuestId = bpq.QuestId,
                            QuestName = bpq.Quest.Name,
                            Quantity = qc.AmountAquired
                        };

            var result = await query.ToListAsync(cancellationToken);
            return result;
        }

        public async Task<List<GetGameSaveInventoryComponentSummaryResponse>> Handle(GetGameSaveInventoryComponentSummaryQuery request, CancellationToken cancellationToken)
        {
            var query = from qc in _repo.QuestComponents.AsNoTracking()
                        join q in _repo.Quests on qc.QuestId equals q.Id
                        where q.GameSaveId == request.GameSaveId
                        group qc by new { qc.ComponentId, qc.Component.Name, qc.Component.Type, qc.Component.Picture.Path } into g
                        select new 
                        {
                            ComponentId = g.Key.ComponentId,
                            ComponentName = g.Key.Name,
                            ComponentType = g.Key.Type,
                            TotalQuantity = g.Sum(qc => qc.AmountAquired),
                            PicturePath = g.Key.Path,
                            Quests = g.Select(qc => new GetGameSaveInventoryComponentQuestDto
                            {
                                Id = qc.QuestId,
                                Name = qc.Quest.Name,
                                Quantity = qc.AmountAquired
                            }).ToList()
                        };

            var rows = await query.ToListAsync(cancellationToken);

            var result = rows.Select(r => new GetGameSaveInventoryComponentSummaryResponse
            {
                ComponentId = r.ComponentId,
                ComponentName = r.ComponentName,
                ComponentType = EnumHelper.GetEnumDescription((ComponentTypeEnum)r.ComponentType),
                TotalQuantity = r.TotalQuantity,
                ComponentImageUrl = ImageHelper.GetFileUrl(r.PicturePath, _baseUrl),
                Quests = r.Quests
            }).ToList();

            return result;
        }

        public async Task<GetGameSaveInventorySummaryResponse> Handle(GetGameSaveInventorySummaryQuery request, CancellationToken cancellationToken)
        {
            var query = from gs in _repo.GameSaves.AsNoTracking()
                        where gs.Id == request.GameSaveId
                        select new GetGameSaveInventorySummaryResponse
                        {
                            Quests = gs.Quests.Count(),
                            ItemStacks = gs.Quests.SelectMany(q => q.QuestComponents).Count(),
                            UniqueItems = gs.Quests
                                .SelectMany(q => q.QuestComponents)
                                .Select(qc => qc.ComponentId)
                                .Distinct()
                                .Count(),
                            TotalItems = gs.Quests.SelectMany(q => q.QuestComponents).Sum(qc => qc.AmountAquired)
                        };
            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if(result == null)
            {
                throw new NotFoundException("Game Save", request.GameSaveId);
            }
            return result;
        }
    }
}
