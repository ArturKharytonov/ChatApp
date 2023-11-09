using Microsoft.AspNetCore.Components;

namespace ChatApp.Domain.DTOs.Http
{
    public class GridModelDto<T> 
        where T : Enum
    {
        [SupplyParameterFromQuery] 
        [Parameter] 
        public int PageNumber { get; set; }
        [SupplyParameterFromQuery]
        [Parameter]
        public string? Data { get; set; }
        [SupplyParameterFromQuery]
        [Parameter]
        public T? Column { get; set; }
        [SupplyParameterFromQuery]
        [Parameter]
        public bool Asc { get; set; } 
        [SupplyParameterFromQuery]
        [Parameter]
        public bool Sorting { get; set; }
    }
}
