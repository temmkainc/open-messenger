using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using MessengerBackend.DTOs;

namespace MessengerBackend.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Server", "Hello from server!");
            await base.OnConnectedAsync();
        }


        //public async Task SendMessageToConversation(int conversationId, string message)
        //{
        //    var userName = Context.User?.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        //    var messageDto = new SendMessageDto
        //    {
        //        SenderId = int.Parse(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)),
        //        Text = message
        //    };

        //    await Clients.Group(conversationId.ToString())
        //                 .SendAsync("ReceiveMessage", userName, messageDto);
        //}

        public async Task JoinConversation(int conversationId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user's ID from the claims
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }
            Console.WriteLine($"Added {Context.ConnectionId} to conversation-{conversationId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task ReceiveMessage(string userName, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userName, message);
        }
    }
}
