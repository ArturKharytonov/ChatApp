using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.Http.Responses
{
    public class AddRoomResponseDto
    {
        public int? CreatedRoomId { get; set; }
        public bool WasAdded { get; set; }
    }
}
