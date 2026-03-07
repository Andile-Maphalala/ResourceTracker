using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pagination.Models;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponents;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponents;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponents;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.GetQuestComponent;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuestComponentController(ISender sender) : Controller
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateQuestComponentResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateQuestComponentResponse>> CreateQuestCompoent([FromBody] CreateQuestComponentCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateQuestComponentsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateQuestComponentsResponse>> CreateQuestCompoentBulk([FromBody] CreateQuestComponentsCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateQuestComponent([FromBody] UpdateQuestComponentCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateQuestComponentBulk([FromBody] UpdateQuestComponentsCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteQuestComponent(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteQuestComponentCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteQuestComponentBulk([FromBody] DeleteQuestComponentsCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetQuestComponentResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetQuestComponentResponse>> GetQuestComponent([FromQuery] GetQuestComponentQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PageableResponse<SearchQuestComponentsResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PageableResponse<SearchQuestComponentsResponse>>> SearchQuestComponent([FromQuery] SearchQuestComponentsQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
