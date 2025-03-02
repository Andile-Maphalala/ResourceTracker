using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.CreateQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.DeleteQuestComponent;
using ResourceTracker.Application.Features.Commands.QuestComponentCommands.UpdateQuestComponent;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.GetQuestComponent;
using ResourceTracker.Application.Features.Queries.QuestComponentQueres.SearchQuestComponent;

namespace ResourceTracker.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuestComponentController(ISender sender) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateQuestCompoent([FromBody] CreateQuestComponentCommand request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuestComponent([FromBody] UpdateQuestComponentCommand request)
        {
            await sender.Send(request);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestComponent([FromBody] DeleteQuestComponentCommand request)
        {
            await sender.Send(request);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestComponent([FromQuery] GetQuestComponentQuery request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> SearchQuestComponent([FromQuery] SearchQuestComponentsQuery request)
        {
            var response = await sender.Send(request);

            return Ok(response);
        }
    }
}
