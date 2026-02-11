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
        public async Task<IActionResult> CreateBuildPlanCompoent([FromBody] CreateBuildPlanComponentCommand request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBuildPlanCompoentBulk([FromBody] CreateBuildPlanComponentsCommand request)
        {
            var response = await sender.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBuildPlanComponent([FromBody] UpdateBuildPlanComponentCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBuildPlanComponentBulk([FromBody] UpdateBuildPlanComponentsCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBuildPlanComponent([FromBody] DeleteBuildPlanComponentCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBuildPlanComponentBulk([FromBody] DeleteBuildPlanComponentsCommand request)
        {
            await sender.Send(request);
            return NoContent();
        }
    }
}
