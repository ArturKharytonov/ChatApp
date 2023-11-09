using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.RoomDto
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MessagesNumber { get; set; }
        public int ParticipantsNumber { get; set; }

    }
}
