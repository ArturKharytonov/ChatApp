using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.Http
{
    public class AddRoomDto
    {
        [SupplyParameterFromQuery]
        [Parameter]
        public string Name { get; set; } = null!;
    }
}
