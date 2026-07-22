using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? ProfilePhoto { get; set; }

        public UserRole Role { get; set; } = UserRole.User;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLogin { get; set; }

        public ICollection<StudentForm> StudentForms { get; set; }
            = new List<StudentForm>();



        public ICollection<ChatConversation> StudentConversations { get; set; }
    = new List<ChatConversation>();

        public ICollection<ChatConversation> AdminConversations { get; set; }
            = new List<ChatConversation>();

        public ICollection<ChatMessage> ChatMessages { get; set; }
            = new List<ChatMessage>();
    }
}