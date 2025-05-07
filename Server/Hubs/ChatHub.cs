using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            if (http.Request.Query.TryGetValue("bookingId", out var bid)
                && int.TryParse(bid, out var bookingId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"booking-{bookingId}");
            }

            if (Context.User.IsInRole("Receptionist"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "receptionists");
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(int bookingId, string message)
        {
            var user = Context.User?.Identity?.Name ?? "Unknown";

            await Clients.Group($"booking-{bookingId}")
                         .SendAsync("ReceiveMessage", bookingId, user, message);

            await Clients.Group("receptionists")
                         .SendAsync("ReceiveMessage", bookingId, user, message);
        }
    }
}
