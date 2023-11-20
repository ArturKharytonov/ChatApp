using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.Http.Responses
{
    public class UpdateMessageResponseDto
    {
        public string Message { get; set; }
        public bool Successful { get; set; }
    }
}
