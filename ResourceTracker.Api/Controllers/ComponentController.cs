using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.ComponentCommands.CreateComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent;
using ResourceTracker.Application.Features.Queries.ComponentQueries.GetComponent;
using ResourceTracker.Application.Features.Queries.ComponentQueries.SearchComponents;

namespace ResourceTracker.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ComponentController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCompoent([FromBody] CreateComponentCommand request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComponent([FromBody] UpdateComponentCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComponent([FromBody] DeleteComponentCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetComponent([FromQuery] GetComponentQuery request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> SearchComponent([FromQuery] SearchComponentsQuery request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }
    }
}
