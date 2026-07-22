using StudentAPI.Models;

namespace StudentAPI.DTOs
{
    public class UpdateUserDto
    {
        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }
    }
}