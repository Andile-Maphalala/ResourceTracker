using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.CreateBuildPlanQuest;
using ResourceTracker.Application.Features.Commands.BuildPlanQuestCommands.DeleteBuildPlanQuest;
using ResourceTracker.Application.Features.Queries.BuildPlanQuestQueries.GetBuildPlanQuestList;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BuildPlanQuestController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateBuildPlanQuestResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateBuildPlanQuestResponse>> CreateBuildPlanQuest([FromBody] CreateBuildPlanQuestCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteBuildPlanQuest([FromQuery] DeleteBuildPlanQuestCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<GetBuildPlanQuestListResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetBuildPlanQuestListResponse>>> GetBuildPlanQuestList([FromQuery] GetBuildPlanQuestListQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
