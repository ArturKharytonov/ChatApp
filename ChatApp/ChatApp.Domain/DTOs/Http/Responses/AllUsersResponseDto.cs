using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.Http.Responses
{
    public class AllUsersResponseDto
    {
        public List<UserDto.UserDto> Users { get; set; }
    }
}
