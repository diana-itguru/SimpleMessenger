using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace SimpleMessenger;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}