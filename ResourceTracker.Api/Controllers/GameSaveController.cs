using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.CreateGameSave;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.DeleteGameSave;
using ResourceTracker.Application.Features.Commands.GameSaveCommands.UpdateGameSave;
using ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSave;
using ResourceTracker.Application.Features.Queries.GameSaveQueries.GetGameSaveList;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameSaveController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateGameSaveResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateGameSaveResponse>> CreateGameSave([FromBody] CreateGameSaveCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateGameSave([FromBody] UpdateGameSaveCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteGameSave(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteGameSaveCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetGameSaveResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetGameSave([FromQuery] GetGameSaveQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<GetGameSaveListResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetGameSaveList([FromQuery] GetGameSaveListQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
