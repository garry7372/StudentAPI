using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public ChatConversation Conversation { get; set; } = null!;

        // Student/Admin who sent the message
        public int SenderId { get; set; }

        public User Sender { get; set; } = null!;

        [Required]
        [MaxLength(3000)]
        public string Message { get; set; } = string.Empty;

        // Seen by receiver?
        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}