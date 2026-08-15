using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pagination.Models;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.CreateBuildPlan;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.DeleteBuildPlan;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.UpdateBuildPlan;
using ResourceTracker.Application.Features.Queries.BuildPlanQueries.GetBuildPlan;
using ResourceTracker.Application.Features.Queries.BuildPlanQueries.SearchBuildPlans;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BuildPlanController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateBuildPlanResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateBuildPlanResponse>> CreateBuildPlan([FromBody] CreateBuildPlanCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateBuildPlan([FromBody] UpdateBuildPlanCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBuildPlan(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteBuildPlanCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetBuildPlanResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetBuildPlanResponse>> GetBuildPlan(int id, CancellationToken cancellationToken)
        {
            var response = await sender.Send(new GetBuildPlanQuery(id), cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PageableResponse<SearchBuildPlansResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PageableResponse<SearchBuildPlansResponse>>> SearchBuildPlan([FromQuery] SearchBuildPlansQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
