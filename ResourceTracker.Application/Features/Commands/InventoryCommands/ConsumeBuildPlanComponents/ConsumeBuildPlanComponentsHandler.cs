

using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Commands.InventoryCommands.ConsumeBuildPlanComponents
{
    public class ConsumeBuildPlanComponentsHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork) : IRequestHandler<ConsumeBuildPlanComponentsCommand, ConsumeBuildPlanComponentsResponse>
    {

        public async Task<ConsumeBuildPlanComponentsResponse> Handle(ConsumeBuildPlanComponentsCommand request, CancellationToken cancellationToken)
        {
            var response = new ConsumeBuildPlanComponentsResponse { Success = true };

            var ids = request.Allocations.Select(a => a.QuestComponentId).ToList();

            var questComponents = await repo.QuestComponents
                .Include(qc => qc.Component)
                .Where(qc => ids.Contains(qc.Id))
                .ToListAsync(cancellationToken);

            foreach (var allocation in request.Allocations)
            {
                var qc = questComponents.FirstOrDefault(x => x.Id == allocation.QuestComponentId);

                if (qc == null)
                {
                    response.Errors.Add($"Inventory record {allocation.QuestComponentId} not found.");
                    continue;
                }

                // A facility is capability, not stock, and belongs in PlayerFacility. Nothing stops
                if (qc.Component != null && qc.Component.Type == (int)ComponentTypeEnum.Facility)
                {
                    response.Errors.Add($"'{qc.Component.Name}' is a facility and is never consumed.");
                    continue;
                }

                if (qc.AmountAquired < allocation.AmountToDeduct)
                {
                    response.Errors.Add($"Insufficient stock for '{qc.ComponentId}' at quest {qc.QuestId} " +
                                         $"(have {qc.AmountAquired}, tried to deduct {allocation.AmountToDeduct}).");
                    continue;
                }

                qc.AmountAquired -= allocation.AmountToDeduct;
            }

            if (response.Errors.Any())
            {
                response.Success = false;
                return response; 
            }

            await unitOfWork.Save(cancellationToken);
            return response;
        }
    }
}
