namespace StudentAPI.DTOs.Chat
{
    public class MessageDto
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public string SenderName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime SentAt { get; set; }
    }
}