using ChatApp.Application.Services.FileService.Interface;
using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Files;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http.Responses.Files;
using ChatApp.Domain.DTOs.Http.Responses.Rooms;
using ChatApp.Domain.Enums;
using ChatApp.UI.Services.OpenAiService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebAPI.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IFileService _fileService;

        private readonly IUserContext _userContext;
        private readonly IOpenAiService _openAiService;
        public RoomController(IRoomService roomService, IUserContext userContext, IOpenAiService openAiService, IFileService fileService)
        {
            _roomService = roomService;
            _userContext = userContext;
            _openAiService = openAiService;
            _fileService = fileService;
        }

        [HttpDelete("by_id")]
        public async Task DeleteRoom([FromQuery] int roomId)
        {
            await _roomService.DeleteRoom(roomId);
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetRooms([FromQuery] GridModelDto<RoomColumnsSorting> model)
        {
            if (int.TryParse(_userContext.GetUserId(), out var id))
                return Ok(await _roomService.GetRoomsPageAsync(id, model));
            return BadRequest();
        }

        [HttpGet("creating")]
        public async Task<IActionResult> CreateRoom([FromQuery] string roomName)
        {
            var id = _userContext.GetUserId();
            if (string.IsNullOrEmpty(id) || await _roomService.DoesRoomExist(roomName)) 
                return BadRequest(new AddRoomResponseDto { WasAdded = false });

            var assistantId = await _openAiService.CreateAssistantAsync(roomName);
            var roomId = await _roomService.CreateRoom(roomName, id, assistantId);

            if (roomId.HasValue)
                return Ok(new AddRoomResponseDto { WasAdded = true, CreatedRoomId = roomId });

            return BadRequest(new AddRoomResponseDto{WasAdded = false});
        }

        [HttpGet]
        public async Task<IActionResult> GetRoom([FromQuery] string id)
        {
            if (!int.TryParse(id, out var roomId)) return BadRequest();

            var room = await _roomService.GetRoom(roomId);
            return Ok(room);
        }

        [HttpGet("by_name")]
        public async Task<IActionResult> GetRoomByName([FromQuery] string name)
        {
            var room = await _roomService.GetRoomByName(name);
            return Ok(room);
        }

        [HttpPost("file")]
        public async Task<IActionResult> PostFileToRoom([FromBody] UploadingRequestDto uploading)
        {
            if(await _fileService.UploadFileAsync(uploading))
                return Ok(new UploadingResponseDto{Message = "File was uploaded"});
            return BadRequest(new UploadingResponseDto { Message = "Bad request" });
        }
    }
}
