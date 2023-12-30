namespace ChatApp.Domain.DTOs.MessageDto
{
    public class MessageDto
    {
        public int Id { get; set; }

        public string Content { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public string RoomName { get; set; } = null!;

        public string SenderUsername { get; set; } = null!;
    }
}
