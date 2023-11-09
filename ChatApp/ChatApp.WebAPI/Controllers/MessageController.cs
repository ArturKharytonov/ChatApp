using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.Enums;
using ChatApp.WebAPI.Services.MessageService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebAPI.Controllers
{
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetPage([FromQuery] GridModelDto<MessageColumnsSorting> model)
        {
            return Ok(await _messageService.GetMessagePageAsync(model));
        }
    }
}
