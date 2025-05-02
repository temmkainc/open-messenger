using MessengerBackend.DTOs;
using MessengerBackend.Hubs;
using MessengerBackend.Models;
using MessengerBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageRepository _messageRepo;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageController(IMessageRepository messageRepo, IHubContext<ChatHub> hubContext)
    {
        _messageRepo = messageRepo;
        _hubContext = hubContext;
    }

    [HttpGet("{conversationId}")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(int conversationId)
    {
        var messages = await _messageRepo.GetMessagesForConversationAsync(conversationId);

        var orderedMessages = messages
            .OrderBy(m => m.SentAt) 
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationID,
                SenderId = m.SenderID,
                Text = m.Text,
                SentAt = m.SentAt,
                Sender = new UserDto
                {
                    Id = m.Sender.Id,
                    Username = m.Sender.Username,
                    Email = m.Sender.Email
                }
            }).ToList();

        return Ok(orderedMessages);
    }


    [HttpPost("{conversationId}")]
    public async Task<ActionResult<Message>> SendMessage(int conversationId, [FromBody] SendMessageDto messageDto)
    {
        if (string.IsNullOrEmpty(messageDto.Text))
        {
            return BadRequest("Message text is required.");
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
        {
            return Unauthorized("Invalid or missing userId in token.");
        }

        var message = await _messageRepo.SendMessageAsync(conversationId, userId, messageDto.Text);

        var messageResponseDto = new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationID,
            SenderId = message.SenderID,
            Text = message.Text,
            SentAt = message.SentAt,
            Sender = message.Sender != null ? new UserDto
            {
                Id = message.Sender.Id,
                Username = message.Sender.Username,
                Email = message.Sender.Email
            } : null
        };

        var sendMessageResponseDto = new SendMessageDto
        {
            SenderId = message.SenderID,
            Text = message.Text,
            SenderUsername = messageResponseDto.Sender.Username
        };

        var userName = User.FindFirstValue(ClaimTypes.Name);
        await _hubContext.Clients.Group(conversationId.ToString())
                                .SendAsync("ReceiveMessage", sendMessageResponseDto);

        return Ok(messageResponseDto);
    }



}
