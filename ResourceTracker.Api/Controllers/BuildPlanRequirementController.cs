using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanSankey;

namespace ResourceTracker.Api.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BuildPlanRequirementController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetBuildPlanRequirementsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetBuildPlanRequirementsResponse>> GetBuildPlanRequirement(int buildPlanId, bool includeFacilityRequirements, bool IncludeInventory, CancellationToken cancellationToken)
        {
            GetBuildPlanRequirementQuery request = new GetBuildPlanRequirementQuery(buildPlanId);
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetBuildPlanSankeyResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetBuildPlanSankeyResponse>> GetBuildPlanSankey(int buildPlanId, CancellationToken cancellationToken)
        {
            GetBuildPlanSankeyQuery request = new GetBuildPlanSankeyQuery(buildPlanId);
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
