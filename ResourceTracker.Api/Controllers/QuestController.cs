using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pagination.Models;
using ResourceTracker.Application.Features.Commands.QuestCommands.CreateQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.DeleteQuest;
using ResourceTracker.Application.Features.Commands.QuestCommands.UpdateQuest;
using ResourceTracker.Application.Features.Queries.QuestQueries.GetQuest;
using ResourceTracker.Application.Features.Queries.QuestQueries.SearchQuests;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuestController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CreateQuestResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateQuestResponse>> CreateQuest([FromBody] CreateQuestCommand request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateQuest([FromBody] UpdateQuestCommand request, CancellationToken cancellationToken)
        {
            await sender.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteQuest(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteQuestCommand(id), cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetQuestResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetQuestResponse>> GetQuest(int id, CancellationToken cancellationToken)
        {
            var response = await sender.Send(new GetQuestQuery(id), cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PageableResponse<SearchQuestsResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PageableResponse<SearchQuestsResponse>>> SearchQuest([FromQuery] SearchQuestsQuery request, CancellationToken cancellationToken)
        {
            var response = await sender.Send(request, cancellationToken);
            return Ok(response);
        }
    }
}
