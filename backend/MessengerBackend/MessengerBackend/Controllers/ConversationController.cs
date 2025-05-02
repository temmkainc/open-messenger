using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using MessengerBackend.Data;
using MessengerBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MessengerBackend.Models;
using MessengerBackend.DTOs;
using Microsoft.AspNetCore.SignalR;
using MessengerBackend.Hubs;

namespace MessengerBackend.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public ConversationController(IConversationRepository conversationRepository, IHubContext<ChatHub> hubContext, IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _conversationRepository = conversationRepository;
            _hubContext = hubContext;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> GetConversations()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Invalid or missing userId in token.");
            }

            var conversations = await _conversationRepository.GetConversationsForUserAsync(userId);

            if (conversations == null)
            {
                return Ok(new List<ConversationDto>()); // return empty list if nothing is found
            }

            var conversationDtos = conversations.Select(c => new ConversationDto
            {
                Id = c.Id,
                Participants = c.Participants?.Select(p => new UserDto
                {
                    Id = p.UserId,
                    Username = p.User?.Username,
                    Email = p.User?.Email
                }).ToList() ?? new List<UserDto>(),

                Messages = c.Messages?.Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationID,
                    SenderId = m.SenderID,
                    Text = m.Text,
                    SentAt = m.SentAt,
                    Sender = new UserDto
                    {
                        Id = m.Sender?.Id ?? 0,
                        Username = m.Sender?.Username,
                        Email = m.Sender?.Email
                    }
                }).ToList() ?? new List<MessageDto>()
            }).ToList();

            return Ok(conversationDtos);
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversation(int conversationId)
        {
            var conversation = await _conversationRepository.GetConversationByIdAsync(conversationId);

            if (conversation == null)
            {
                return NotFound("Conversation not found.");
            }

            var conversationDto = new ConversationDto
            {
                Id = conversation.Id,
                Participants = conversation.Participants.Select(p => new UserDto
                {
                    Id = p.UserId,
                    Username = p.User.Username,
                    Email = p.User.Email
                }).ToList(),
                Messages = conversation.Messages.Select(m => new MessageDto
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
                }).ToList()
            };

            return Ok(conversation);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto conversationDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var currentUserId))
            {
                return Unauthorized("Invalid or missing userId in token.");
            }

            try
            {
                var participantIds = conversationDto.ParticipantIds.Distinct().ToList();
                if (!participantIds.Contains(currentUserId))
                {
                    participantIds.Add(currentUserId);
                }

                var participants = participantIds.Select(id => new UserConversation
                {
                    UserId = id
                }).ToList();

                var conversation = new Conversation
                {
                    Messages = new List<Message>(),
                    Participants = participants
                };

                foreach (var participant in participants)
                {
                    participant.Conversation = conversation;
                }

                await _conversationRepository.CreateConversationAsync(conversation);

                var users = await _userRepository.GetUsersByIdsAsync(participantIds);

                var conversationDtoResult = new ConversationDto
                {
                    Id = conversation.Id,
                    Participants = users.Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email
                    }).ToList(),
                    Messages = new List<MessageDto>() 
                };

                return Ok(conversationDtoResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the conversation: {ex.Message}");
            }
        }


        [HttpPost("{conversationId}/addUser/{userId}")]
        public async Task<IActionResult> AddUserToConversation(int conversationId, int userId)
        {
            await _conversationRepository.AddUserToConversationAsync(conversationId, userId);
            await _hubContext.Clients.Group(conversationId.ToString()).SendAsync("UserJoined", userId);


            return Ok("User added to conversation.");
        }

    }
}
