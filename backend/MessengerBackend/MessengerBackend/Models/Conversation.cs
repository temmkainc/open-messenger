namespace MessengerBackend.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public ICollection<UserConversation>? Participants { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
