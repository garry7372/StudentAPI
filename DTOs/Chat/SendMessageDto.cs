using System.ComponentModel.DataAnnotations;

namespace StudentAPI.DTOs.Chat
{
    public class SendMessageDto
    {
        [Required]
        [MaxLength(3000)]
        public string Message { get; set; } = string.Empty;
    }
}