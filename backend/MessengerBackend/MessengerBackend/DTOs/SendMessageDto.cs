namespace MessengerBackend.DTOs
{
    public class SendMessageDto
    {
        public string Text { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername {  get; set; }
    }
}
