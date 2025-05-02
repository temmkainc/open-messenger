namespace MessengerBackend.DTOs
{
    public class CreateConversationDto
    {
        public List<int> ParticipantIds { get; set; } // List of user IDs for the participants
    }
}
