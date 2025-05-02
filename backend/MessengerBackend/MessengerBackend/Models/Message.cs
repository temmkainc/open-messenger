namespace MessengerBackend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationID { get; set; }

        public int SenderID { get; set; }
        public User Sender { get; set; }

        public string Text { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
