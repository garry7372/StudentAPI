using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAPI.DTOs.Chat;
using StudentAPI.Interfaces;
using System.Security.Claims;

namespace StudentAPI.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        //==========================================
        // Get My Messages
        //==========================================

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            int studentId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var messages = await _chatService.GetMessagesAsync(studentId);

            return Ok(messages);
        }

        //==========================================
        // Send Message
        //==========================================

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(
            SendMessageDto dto)
        {
            int studentId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _chatService.SendMessageAsync(
                studentId,
                dto.Message);

            return Ok(new
            {
                message = "Message sent successfully."
            });
        }

        //==========================================
        // Mark Read
        //==========================================

        [HttpPut("read")]
        public async Task<IActionResult> MarkRead()
        {
            int studentId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _chatService.MarkAsReadAsync(studentId);

            return Ok();
        }
    }
}