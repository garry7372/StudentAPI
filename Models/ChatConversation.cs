using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models
{
    public class ChatConversation
    {
        public int Id { get; set; }

        // Student
        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        // Admin who replied
        public int? AdminId { get; set; }
        public User? Admin { get; set; }

        public ConversationStatus Status { get; set; } = ConversationStatus.Open;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

        // Automatically delete after archive/close
        public DateTime? ExpiresAt { get; set; }

        public ICollection<ChatMessage> Messages { get; set; }
            = new List<ChatMessage>();
    }
}