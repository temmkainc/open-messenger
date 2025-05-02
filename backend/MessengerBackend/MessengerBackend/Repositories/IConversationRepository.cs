using MessengerBackend.Models;

namespace MessengerBackend.Repositories
{
    public interface IConversationRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsForUserAsync(int userId);
        Task<Conversation?> GetConversationByIdAsync(int conversationId);
        Task CreateConversationAsync(Conversation conversation);
        Task AddUserToConversationAsync(int conversationId, int userId);
    }
}
