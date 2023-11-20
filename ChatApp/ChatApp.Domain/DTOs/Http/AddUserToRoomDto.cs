using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.Http
{
    public class AddUserToRoomDto
    {
        public string UserId { get; set; }
        public string RoomId { get; set; }
    }
}
