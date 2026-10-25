using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.RecipeCommands.CreateRecipe;
using ResourceTracker.Application.Features.Commands.RecipeCommands.DeleteRecipe;
using ResourceTracker.Application.Features.Commands.RecipeCommands.UpdateRecipe;
using ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipe;
using ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipesForComponent;

namespace ResourceTracker.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RecipeController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateRecipeResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateRecipeResponse>> CreateRecipe([FromBody] CreateRecipeCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateRecipe([FromBody] UpdateRecipeCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteRecipe(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteRecipeCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetRecipeResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetRecipeResponse>> GetRecipe(int id, CancellationToken cancellationToken)
        {
            var response = await sender.Send(new GetRecipeQuery(id), cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Recipes whose output is the given component. At most one today, but returned as a list.
        /// </summary>
        [HttpGet("{componentId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<GetRecipeResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetRecipeResponse>>> GetRecipesForComponent(int componentId, CancellationToken cancellationToken)
        {
            var response = await sender.Send(new GetRecipesForComponentQuery(componentId), cancellationToken);
            return Ok(response);
        }
    }
}
