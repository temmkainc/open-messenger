namespace MessengerBackend.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
        public UserDto? Sender { get; set; } 
    }

}
