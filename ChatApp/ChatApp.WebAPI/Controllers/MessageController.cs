using ChatApp.Application.Services.MessageService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.Enums;
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

        [HttpPost]
        public async Task<IActionResult> AddMessageAsync([FromBody] AddMessageDto message)
        {
            var messageDto = await _messageService.AddMessageAsync(message);
            return Ok(messageDto);
        }

        [HttpGet("all/{roomId}")]
        public async Task<IActionResult> GetAllMessages(string roomId)
        {
            return Ok(await _messageService.GetMessagesFromChat(roomId));
        }
    }
}
