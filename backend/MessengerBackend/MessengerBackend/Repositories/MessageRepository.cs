using MessengerBackend.Data;
using MessengerBackend.Models;
using MessengerBackend.Repositories;
using Microsoft.EntityFrameworkCore;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetMessagesForConversationAsync(int conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationID == conversationId)
            .Include(m => m.Sender)
            .ToListAsync();
    }

    public async Task<Message> SendMessageAsync(int conversationId, int senderId, string text)
    {
        var message = new Message
        {
            ConversationID = conversationId,
            SenderID = senderId,
            Text = text,
            SentAt = DateTime.UtcNow,
            Sender = await _context.Users
                            .Where(u => u.Id == senderId)
                            .FirstOrDefaultAsync()
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();


        return message;
    }
}
