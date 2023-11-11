using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.Http
{
    public class AddMessageDto
    {
        public string Content { get; set; }
        public int RoomId { get; set; }
        public string UserId { get; set; }
        public DateTime SentAt { get; set; }
    }
}
