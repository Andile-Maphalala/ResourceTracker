using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateQuest([FromBody] CreateQuestCommand request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuest([FromBody] UpdateQuestCommand request)
        {
            await sender.Send(request);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuest([FromBody] DeleteQuestCommand request)
        {
            await sender.Send(request);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetQuest([FromQuery] GetQuestQuery request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> SearchQuest([FromQuery] SearchQuestsQuery request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }
    }
}
