using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventoryComponentSummary.Dto;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventorySummary;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventoryTotals;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetInventoryComponentQuest;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Queries.InventoryQueries
{
    public class InventoryQueries(IResourceTrackerRepository repo) : IRequestHandler<GetInventoryComponentQuestQuery, List<GetInventoryComponentQuestResponse>>,
                                                                    IRequestHandler<GetGameSaveInventoryComponentSummaryQuery, List<GetGameSaveInventoryComponentSummaryResponse>>,
                                                                    IRequestHandler<GetGameSaveInventorySummaryQuery, GetGameSaveInventorySummaryResponse>
    {

        public async Task<List<GetInventoryComponentQuestResponse>> Handle(GetInventoryComponentQuestQuery request, CancellationToken cancellationToken)
        {
            var query = from qc in repo.QuestComponents.AsNoTracking()
                        join bpq in repo.BuildPlanQuests on qc.QuestId equals bpq.QuestId
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
            var query = from qc in repo.QuestComponents.AsNoTracking()
                        join q in repo.Quests on qc.QuestId equals q.Id
                        where q.GameSaveId == request.GameSaveId
                        group qc by new { qc.ComponentId, qc.Component.Name, qc.Component.Type } into g
                        select new GetGameSaveInventoryComponentSummaryResponse
                        {
                            ComponentId = g.Key.ComponentId,
                            ComponentName = g.Key.Name,
                            ComponentType = EnumHelper.GetEnumDescription((ComponentTypeEnum)g.Key.Type),
                            TotalQuantity = g.Sum(qc => qc.AmountAquired),
                            Quests = g.Select(qc => new GetGameSaveInventoryComponentQuestDto
                            {
                                Id = qc.QuestId,
                                Name = qc.Quest.Name,
                                Quantity = qc.AmountAquired
                            }).ToList()
                        };

            var result = await query.ToListAsync(cancellationToken);
            return result;
        }

        public async Task<GetGameSaveInventorySummaryResponse> Handle(GetGameSaveInventorySummaryQuery request, CancellationToken cancellationToken)
        {
            var query = from gs in repo.GameSaves.AsNoTracking()
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
