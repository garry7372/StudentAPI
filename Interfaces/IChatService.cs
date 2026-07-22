using StudentAPI.DTOs;
using StudentAPI.DTOs.Chat;

namespace StudentAPI.Interfaces
{
    public interface IChatService
    {
        Task SendMessageAsync(int studentId, string message);

        Task ReplyAsync(int adminId, int studentId, string message);

        Task<List<MessageDto>> GetMessagesAsync(int studentId);

        Task<List<ConversationDto>> GetConversationsAsync();

        Task MarkAsReadAsync(int studentId);

        Task<List<ConversationDto>> SearchConversationAsync(string keyword);

        Task<int> GetUnreadCountAsync();



        Task ClearAllChatsAsync();

        Task DeleteOldChatsAsync();
    }
}