namespace MessengerBackend.DTOs
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public List<UserDto> Participants { get; set; }
        public List<MessageDto> Messages { get; set; }
    }
}
