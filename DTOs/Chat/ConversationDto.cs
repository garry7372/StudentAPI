namespace StudentAPI.DTOs
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }

        public int StudentId { get; set; }

        public string StudentName { get; set; } = "";

        public string Email { get; set; } = "";

        public string LastMessage { get; set; } = "";

        // Last message date/time
        public DateTime LastMessageTime { get; set; }

        // Number of unread student messages
        public int UnreadCount { get; set; }

        // Red dot in admin list
        public bool HasUnreadMessages { get; set; }

        // Open / Closed
        public string Status { get; set; } = "";
    }
}