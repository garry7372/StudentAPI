namespace StudentAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string? ProfilePhoto { get; set; }

        public string Role { get; set; } = "";

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}