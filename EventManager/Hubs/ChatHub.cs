using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EventManagementSystem.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToGroup(string eventId, string message)
        {
            var user = Context.User.Identity.Name;

            await Clients.Group(eventId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddToGroup(string eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, eventId);
        }
    }
}