using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.CreatePlayerFacility;
using ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.DeletePlayerFacility;
using ResourceTracker.Application.Features.Commands.PlayerFacilityCommands.UpdatePlayerFacility;
using ResourceTracker.Application.Features.Queries.PlayerFacilityQueries.GetPlayerFacility;
using ResourceTracker.Application.Features.Queries.PlayerFacilityQueries.GetPlayerFacilitiesByQuest;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlayerFacilityController(ISender sender) : Controller
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreatePlayerFacilityResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreatePlayerFacilityResponse>> CreatePlayerFacility([FromBody] CreatePlayerFacilityCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePlayerFacility([FromBody] UpdatePlayerFacilityCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePlayerFacility(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeletePlayerFacilityCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetPlayerFacilityResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetPlayerFacilityResponse>> GetPlayerFacility(int id, CancellationToken cancellationToken)
        {
            var response = await sender.Send(new GetPlayerFacilityQuery(id), cancellationToken);
            return Ok(response);
        }

        [HttpGet("{questId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<GetPlayerFacilitiesByQuestResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetPlayerFacilitiesByQuestResponse>>> GetPlayerFacilitiesByQuest(int questId, CancellationToken cancellationToken)
        {
            var response = await sender.Send(new GetPlayerFacilitiesByQuestQuery(questId), cancellationToken);
            return Ok(response);
        }
    }
}
