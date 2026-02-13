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
        public async Task<IActionResult> CreateGameSave([FromBody] CreateGameSaveCommand request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGameSave([FromBody] UpdateGameSaveCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGameSave([FromBody] DeleteGameSaveCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetGameSave([FromQuery] GetGameSaveQuery request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetGameSaveList([FromQuery] GetGameSaveListQuery request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }
    }
}
