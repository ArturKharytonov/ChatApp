using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.Enums;
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
        private readonly IUserContext _userContext;
        public RoomController(IRoomService roomService, IUserContext userContext)
        {
            _roomService = roomService;
            _userContext = userContext;
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
            if (string.IsNullOrEmpty(id)) 
                return BadRequest(new AddRoomResponseDto { WasAdded = false });

            var roomId = await _roomService.CreateRoom(roomName, id);

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
    }
}
