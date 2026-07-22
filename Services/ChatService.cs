using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.DTOs;
using StudentAPI.DTOs.Chat;
using StudentAPI.Interfaces;
using StudentAPI.Models;

namespace StudentAPI.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        //====================================================
        // Student Send Message
        //====================================================

        public async Task SendMessageAsync(int studentId, string message)
        {
            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(x => x.StudentId == studentId);

            if (conversation == null)
            {
                conversation = new ChatConversation
                {
                    StudentId = studentId,
                    Status = ConversationStatus.Open,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LastMessageAt = DateTime.UtcNow
                };

                _context.ChatConversations.Add(conversation);

                await _context.SaveChangesAsync();
            }

            var chatMessage = new ChatMessage
            {
                ConversationId = conversation.Id,
                SenderId = studentId,
                Message = message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(chatMessage);

            conversation.LastMessageAt = DateTime.UtcNow;
            conversation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        //====================================================
        // Admin Reply
        //====================================================

        public async Task ReplyAsync(int adminId, int studentId, string message)
        {
            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(x => x.StudentId == studentId);

            if (conversation == null)
                throw new Exception("Conversation not found.");

            conversation.AdminId = adminId;

            conversation.UpdatedAt = DateTime.UtcNow;

            conversation.LastMessageAt = DateTime.UtcNow;

            var chat = new ChatMessage
            {
                ConversationId = conversation.Id,
                SenderId = adminId,
                Message = message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(chat);

            await _context.SaveChangesAsync();
        }

        //====================================================
        // Student/Admin Get Messages
        //====================================================

        public async Task<List<MessageDto>> GetMessagesAsync(int studentId)
        {
            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(x => x.StudentId == studentId);

            if (conversation == null)
                return new List<MessageDto>();

            return await _context.ChatMessages
                .Where(x => x.ConversationId == conversation.Id)
                .OrderBy(x => x.SentAt)
                .Select(x => new MessageDto
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    SenderName = x.Sender.FullName,
                    Message = x.Message,
                    IsRead = x.IsRead,
                    SentAt = x.SentAt
                })
                .ToListAsync();
        }

        //====================================================
        // Admin Conversation List
        //====================================================

        public async Task<List<ConversationDto>> GetConversationsAsync()
        {
            return await _context.ChatConversations
                .Include(c => c.Student)
                .Include(c => c.Messages)
                .OrderByDescending(c => c.LastMessageAt)
                .Select(c => new ConversationDto
                {
                    ConversationId = c.Id,

                    StudentId = c.StudentId,

                    StudentName = c.Student.FullName,

                    LastMessage = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.Message)
                        .FirstOrDefault() ?? "",

                    LastMessageTime = c.LastMessageAt,

                    HasUnreadMessages = c.Messages.Any(m =>
                        !m.IsRead &&
                        m.SenderId == c.StudentId),

                    Status = c.Status.ToString()
                })
                .ToListAsync();
        }
        //====================================================
        // Mark Messages Read
        //====================================================

        public async Task MarkAsReadAsync(int studentId)
        {
            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(x => x.StudentId == studentId);

            if (conversation == null)
                return;

            var messages = await _context.ChatMessages
                .Where(x =>
                    x.ConversationId == conversation.Id &&
                    !x.IsRead)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }



        public async Task<int> GetUnreadCountAsync()
        {
            return await _context.ChatMessages
                .Include(x => x.Sender)
                .CountAsync(x =>
                    !x.IsRead &&
                    x.Sender.Role == UserRole.User);
        }



        public async Task<List<ConversationDto>> SearchConversationAsync(string keyword)
        {
            keyword = keyword.ToLower();

            return await _context.ChatConversations
                .Include(c => c.Student)
                .Include(c => c.Messages)
                .Where(c =>
                    c.Student.FullName.ToLower().Contains(keyword) ||
                    c.Student.Email.ToLower().Contains(keyword))
                .Select(c => new ConversationDto
                {
                    ConversationId = c.Id,

                    StudentId = c.StudentId,

                    StudentName = c.Student.FullName,

                    Email = c.Student.Email,

                    LastMessage = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.Message)
                        .FirstOrDefault() ?? "",

                    LastMessageTime = c.LastMessageAt,

                    UnreadCount = c.Messages.Count(m =>
                        !m.IsRead &&
                        m.Sender.Role == UserRole.User)
                })
                .ToListAsync();
        }


        public async Task ClearAllChatsAsync()
        {
            _context.ChatMessages.RemoveRange(_context.ChatMessages);

            _context.ChatConversations.RemoveRange(_context.ChatConversations);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteOldChatsAsync()
        {
            var setting = await _context.ChatSettings.FirstOrDefaultAsync();

            if (setting == null)
                return;

            var deleteBefore = DateTime.UtcNow.AddDays(-setting.DeleteAfterDays);

            var oldMessages = await _context.ChatMessages
                .Where(x => x.SentAt < deleteBefore)
                .ToListAsync();

            _context.ChatMessages.RemoveRange(oldMessages);

            await _context.SaveChangesAsync();
        }
    }
}
