using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.Http.Responses
{
    public class UserGroupsResponseDto
    {
        public List<string> GroupsId { get; set; } = new List<string>();
    }
}
