namespace ChatApp.Domain.DTOs.Http.Responses.Common;

public class GridModelResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
}