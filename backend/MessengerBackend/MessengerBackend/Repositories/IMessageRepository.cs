using MessengerBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesForConversationAsync(int conversationId);
        Task<Message> SendMessageAsync(int conversationId, int senderId, string text);
    }

}
