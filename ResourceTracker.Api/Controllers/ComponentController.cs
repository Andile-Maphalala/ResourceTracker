using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.Models;
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
        [ProducesResponseType(typeof(CreateComponentResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateComponentResponse>> CreateCompoent([FromBody] CreateComponentCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateComponent([FromBody] UpdateComponentCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteComponent(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteComponentCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetComponentResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetComponentResponse>> GetComponent([FromQuery] GetComponentQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PageableResponse<SearchComponentsResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PageableResponse<SearchComponentsResponse>>> SearchComponent([FromQuery] SearchComponentsQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
