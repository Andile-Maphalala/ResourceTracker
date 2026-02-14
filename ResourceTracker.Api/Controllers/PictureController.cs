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
        public async Task<IActionResult> CreatePicture([FromForm] CreatePictureCommand request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePicture([FromBody] DeletePictureCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetPicture([FromQuery] GetPictureQuery request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }
    }
}
