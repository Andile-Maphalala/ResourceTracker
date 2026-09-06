using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.InventoryCommands.ConsumeBuildPlanComponents;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventorySummary;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetGameSaveInventoryTotals;
using ResourceTracker.Application.Features.Queries.InventoryQueries.GetInventoryComponentQuest;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InventoryController(ISender sender) : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(typeof(List<GetInventoryComponentQuestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetInventoryComponentQuestResponse>> GetInventoryComponentQuest(int componentId, int buildPlanId, CancellationToken cancellationToken)
        {
            GetInventoryComponentQuestQuery request = new GetInventoryComponentQuestQuery(componentId, buildPlanId);
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<GetGameSaveInventoryComponentSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetGameSaveInventoryComponentSummaryResponse>> GetGameSaveInventoryComponentSummary(int gameSaveId, CancellationToken cancellationToken)
        {
            GetGameSaveInventoryComponentSummaryQuery request = new GetGameSaveInventoryComponentSummaryQuery(gameSaveId);
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetGameSaveInventorySummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetGameSaveInventorySummaryResponse>> GetGameSaveInventorySummary(int gameSaveId, CancellationToken cancellationToken)
        {
            GetGameSaveInventorySummaryQuery request = new GetGameSaveInventorySummaryQuery(gameSaveId);
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(typeof(List<ConsumeBuildPlanComponentsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConsumeBuildPlanComponentsResponse>> ConsumeBuildPlanComponents([FromBody] ConsumeBuildPlanComponentsCommand command, CancellationToken cancellationToken)
        {
            var response = await sender.Send(command, cancellationToken);
            return Ok(response);
        }

    }
}   
