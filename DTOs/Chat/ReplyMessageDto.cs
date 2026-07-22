using System.ComponentModel.DataAnnotations;

namespace StudentAPI.DTOs.Chat
{
    public class ReplyMessageDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        [MaxLength(3000)]
        public string Message { get; set; } = string.Empty;
    }
}