using MessengerBackend.Data;
using MessengerBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _context;

        public ConversationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsForUserAsync(int userId)
        {
            return await _context.Conversations
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .Include(c => c.Participants)
                    .ThenInclude(cp => cp.User)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Sender)
                .ToListAsync();
        }

        public async Task<Conversation?> GetConversationByIdAsync(int conversationId)
        {
            return await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == conversationId);
        }

        public async Task CreateConversationAsync(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserToConversationAsync(int conversationId, int userId)
        {
            var userConversation = new UserConversation
            {
                UserId = userId,
                ConversationId = conversationId
            };

            await _context.UserConversations.AddAsync(userConversation);
            await _context.SaveChangesAsync();
        }


    }
}
