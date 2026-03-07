using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.PictureCommands.CreatePicture;
using ResourceTracker.Application.Features.Commands.PictureCommands.DeletePicture;
using ResourceTracker.Application.Features.Queries.PictureQueries.GetPicture;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PictureController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreatePictureResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> CreatePicture([FromForm] CreatePictureCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePicture([FromBody] DeletePictureCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetPictureResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetPicture([FromQuery] GetPictureQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
