namespace ChatApp.Domain.DTOs.Http.Requests.Messages
{
    public class AddMessageDto
    {
        public string Content { get; set; }
        public int RoomId { get; set; }
        public string UserId { get; set; }
        public DateTime SentAt { get; set; } // maybe in future to change on DateTimeOffset at all message models
    }
}
