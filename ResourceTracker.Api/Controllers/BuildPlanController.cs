using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.CreateBuildPlan;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.DeleteBuildPlan;
using ResourceTracker.Application.Features.Commands.BuildPlanCommands.UpdateBuildPlan;
using ResourceTracker.Application.Features.Queries.BuildPlanQueries.GetBuildPlan;
using ResourceTracker.Application.Features.Queries.BuildPlanQueries.SearchBuildPlans;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildPlanController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateBuildPlan([FromBody] CreateBuildPlanCommand request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBuildPlan([FromBody] UpdateBuildPlanCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBuildPlan([FromBody] DeleteBuildPlanCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetBuildPlan([FromQuery] GetBuildPlanQuery request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> SearchBuildPlan([FromQuery] SearchBuildPlansQuery request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }
    }
}
