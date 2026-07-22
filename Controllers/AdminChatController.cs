using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAPI.DTOs.Chat;
using StudentAPI.Interfaces;
using System.Security.Claims;

namespace StudentAPI.Controllers
{
    [Route("api/admin/chat")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public AdminChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        //==========================================
        // All Conversations
        //==========================================

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var data =
                await _chatService.GetConversationsAsync();

            return Ok(data);
        }

        //==========================================
        // Open Student Conversation
        //==========================================

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetMessages(
            int studentId)
        {
            var messages =
                await _chatService.GetMessagesAsync(studentId);

            return Ok(messages);
        }

        //==========================================
        // Reply
        //==========================================

        [HttpPost("reply")]
        public async Task<IActionResult> Reply(
            ReplyMessageDto dto)
        {
            int adminId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _chatService.ReplyAsync(
                adminId,
                dto.StudentId,
                dto.Message);

            return Ok(new
            {
                message = "Reply sent successfully."
            });
        }

        //==========================================
        // Mark Conversation Read
        //==========================================

        [HttpPut("read/{studentId}")]
        public async Task<IActionResult> Read(
            int studentId)
        {
            await _chatService.MarkAsReadAsync(studentId);

            return Ok();
        }



        [HttpGet("unread-count")]
        public async Task<IActionResult> UnreadCount()
        {
            var count = await _chatService.GetUnreadCountAsync();

            return Ok(new
            {
                unread = count
            });
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword)
        {
            return Ok(await _chatService.SearchConversationAsync(keyword));
        }


        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAll()
        {
            await _chatService.ClearAllChatsAsync();

            return Ok(new
            {
                message = "All chats cleared successfully."
            });
        }
    }
}