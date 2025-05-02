using Newtonsoft.Json;

namespace MessengerBackend.Models
{
    public class UserConversation
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ConversationId { get; set; }

        [JsonIgnore]
        public Conversation Conversation { get; set; }
    }
}
