using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.GameCommands.CreateGame;
using ResourceTracker.Application.Features.Commands.GameCommands.DeleteGame;
using ResourceTracker.Application.Features.Commands.GameCommands.UpdateGame;
using ResourceTracker.Application.Features.Queries.GameQueries.GetGame;
using ResourceTracker.Application.Features.Queries.GameQueries.SearchGames;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameCommand request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGame([FromBody] UpdateGameCommand request)
        {
            await sender.Send(request);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGame([FromBody] DeleteGameCommand request)
        {
            await sender.Send(request);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetGame([FromQuery] GetGameQuery request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> SearchGame([FromQuery] SearchGamesQuery request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }
    }
}
