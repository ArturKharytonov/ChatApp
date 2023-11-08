using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.WebAPI.Services.RoomService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebAPI.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("page")]
        public IActionResult GetRooms([FromQuery] GridModelDto<RoomColumnsSorting> model)
        {
            return Ok(_roomService.GetRoomsPage(model));
        }
    }
}
