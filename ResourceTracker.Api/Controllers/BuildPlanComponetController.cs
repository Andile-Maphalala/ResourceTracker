using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.CreateBuildPlanComponents;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.DeleteBuildPlanComponents;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponent;
using ResourceTracker.Application.Features.Commands.BuildPlanComponentCommands.UpdateBuildPlanComponents;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BuildPlanComponetController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateBuildPlanComponentResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateBuildPlanComponentResponse>> CreateBuildPlanCompoent([FromBody] CreateBuildPlanComponentCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateBuildPlanComponentsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateBuildPlanComponentsResponse>> CreateBuildPlanCompoentBulk([FromBody] CreateBuildPlanComponentsCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateBuildPlanComponent([FromBody] UpdateBuildPlanComponentCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateBuildPlanComponentBulk([FromBody] UpdateBuildPlanComponentsCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteBuildPlanComponent(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteBuildPlanComponentCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteBuildPlanComponentBulk([FromBody] DeleteBuildPlanComponentsCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }
    }
}
