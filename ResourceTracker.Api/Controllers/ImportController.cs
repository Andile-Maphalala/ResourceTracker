using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.ImportCommands.ImportGame;

namespace ResourceTracker.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImportController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ImportGameComponents([FromForm] ImportGameCommand request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }
    }
}
