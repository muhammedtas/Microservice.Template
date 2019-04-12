using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace IntradayDashboard.WebApi.Hubs
{
    public class BackgroundJobHub: Hub
    {
         public async Task SendToAll(string user, string message)
        {
            await Clients.All.SendAsync("sendToAll", user, message);
        }
        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "Group Users" };
            return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {   
            await Groups.AddToGroupAsync(Context.ConnectionId, "Group Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Group Users");
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}