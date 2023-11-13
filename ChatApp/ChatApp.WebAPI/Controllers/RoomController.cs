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
            int.TryParse(_userContext.GetUserId(), out var id);
            return Ok(await _roomService.GetRoomsPageAsync(id, model));
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoom([FromQuery] AddRoomDto room)
        {
            int.TryParse(_userContext.GetUserId(), out var id);
            var roomId = await _roomService.CreateRoom(room.Name);
            if (roomId.HasValue)
            {
                //TODO: add user to userAndRooms
                return Ok(new AddRoomResponseDto { WasAdded = true });
            }
            return BadRequest(new AddRoomResponseDto{WasAdded = false});
        }
    }
}
