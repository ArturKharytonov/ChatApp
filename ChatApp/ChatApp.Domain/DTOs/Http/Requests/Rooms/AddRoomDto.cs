﻿using Microsoft.AspNetCore.Components;

namespace ChatApp.Domain.DTOs.Http.Requests.Rooms
{
    public class AddRoomDto
    {
        [SupplyParameterFromQuery]
        [Parameter]
        public string Name { get; set; } = null!;

        [SupplyParameterFromQuery]
        [Parameter]
        public string AssistantId { get; set; } = null!;
    }
}
