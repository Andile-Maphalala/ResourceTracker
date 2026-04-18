using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.PictureCommands.CreatePicture;
using ResourceTracker.Application.Features.Commands.PictureCommands.DeletePicture;
using ResourceTracker.Application.Features.Queries.PictureQueries.GetPicture;
using ResourceTracker.Application.Features.Queries.PictureQueries.GetPictureList;

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
        public async Task<IActionResult> DeletePicture(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeletePictureCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetPictureResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetPictureResponse>> GetPicture([FromQuery] GetPictureQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<GetPictureListResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetPictureListResponse>>> GetPictureList([FromQuery] GetPictureListQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
