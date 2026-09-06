

using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Interfaces;

namespace ResourceTracker.Application.Features.Commands.InventoryCommands.ConsumeBuildPlanComponents
{
    public class ConsumeBuildPlanComponentsHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork) : IRequestHandler<ConsumeBuildPlanComponentsCommand, ConsumeBuildPlanComponentsResponse>
    {

        public async Task<ConsumeBuildPlanComponentsResponse> Handle(ConsumeBuildPlanComponentsCommand request, CancellationToken cancellationToken)
        {
            var response = new ConsumeBuildPlanComponentsResponse { Success = true };

            var ids = request.Allocations.Select(a => a.QuestComponentId).ToList();

            var questComponents = await repo.QuestComponents
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
