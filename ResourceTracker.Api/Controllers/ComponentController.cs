using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.ComponentCommands.CreateComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.DeleteComponent;
using ResourceTracker.Application.Features.Commands.ComponentCommands.UpdateComponent;
using System.Runtime.InteropServices;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ComponentController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCompoent(CreateComponentCommand request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComponent(UpdateComponentCommand request)
        {
            await sender.Send(request);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComponent(DeleteComponentCommand request)
        {
            await sender.Send(request);

            return NoContent();
        }
    }
}
