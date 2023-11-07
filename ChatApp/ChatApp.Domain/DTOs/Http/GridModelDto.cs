namespace ChatApp.Domain.DTOs.Http
{
    public class GridModelDto<T> 
        where T : Enum
    {
        public int PageNumber { get; set; }
        public string? Data { get; set; }
        public T? Column { get; set; }
        public bool Asc { get; set; }
        public bool Sorting { get; set; }
    }
}
