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
        [ProducesResponseType(typeof(ImportGameResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ImportGameComponents([FromForm] ImportGameCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
