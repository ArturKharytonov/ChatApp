namespace ChatApp.Domain.DTOs.Http.Responses.Rooms
{
    public class AddRoomResponseDto
    {
        public int? CreatedRoomId { get; set; }
        public bool WasAdded { get; set; }
    }
}
