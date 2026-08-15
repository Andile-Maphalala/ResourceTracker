using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Queries.BuildPlanRequirementQueries.GetBuildPlanRequirement;

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
            GetBuildPlanRequirementQuery request = new GetBuildPlanRequirementQuery
            {
                BuildPlanId = buildPlanId,
                IncludeFacilityRequirements = includeFacilityRequirements,
                IncludeInventory = IncludeInventory
            };
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
