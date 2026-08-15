using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.Models;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame;
using ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame;
using ResourceTracker.Application.Features.Queries.GameQueries.GetGame;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;

namespace ResourceTracker.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateGameResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateGame([FromForm] CreateGameCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateGameWithImageResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateGameWithImage([FromForm] CreateGameWithImageCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateGame([FromBody] UpdateGameCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteGame(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteGameCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetGameResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetGameResponse>> GetGame(int id, CancellationToken cancellationToken)
        {
            var response = await sender.Send(new GetGameQuery(id), cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PageableResponse<SearchGamesResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PageableResponse<SearchGamesResponse>>> SearchGame([FromQuery] SearchGamesQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
