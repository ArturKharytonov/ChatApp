﻿using ChatApp.Application.Services.MessageService.Interfaces;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using Microsoft.AspNetCore.Authorization;
using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Messages;
using ChatApp.Domain.DTOs.Http.Responses.Messages;

namespace ChatApp.WebAPI.Controllers
{
    [Route("api/message")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpDelete("in_room")]
        public async Task DeleteMessagesInRoomAsync([FromQuery] int roomId)
        {
            await _messageService.DeleteAllMessagesInRoomAsync(roomId);
        }

        [HttpDelete]
        public async Task DeleteMessageAsync([FromQuery] int messageId)
        {
            await _messageService.DeleteMessageAsync(messageId);
        }
        
        [HttpGet("page")]
        public async Task<IActionResult> GetPageAsync([FromQuery] GridModelDto<MessageColumnsSorting> model)
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
        public async Task<IActionResult> GetAllMessagesAsync([FromRoute] string roomId)
        {
            return Ok(await _messageService.GetMessagesFromChat(roomId));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMessageAsync([FromBody] MessageDto messageDto)
        {
            if (await _messageService.UpdateMessageAsync(messageDto))
            {
                return Ok(new UpdateMessageResponseDto
                {
                    Successful = true,
                    Message = "Updated successfully"
                });
            }
            return BadRequest(new UpdateMessageResponseDto
            {
                Successful = false,
                Message = "Smth went wrong"
            });
        }
    }
}
