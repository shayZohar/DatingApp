using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if(isOnline)
            {
                // sending message to all other connected user to notify them that the user is connected
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            }

            var currentUsers = await _tracker.GetOnlineUsers();
            // sending the users online to all of the clients online
            await Clients.Caller.SendAsync("GetOnlineUsers",currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconneted(Context.User.GetUsername(), Context.ConnectionId);
            if(isOffline)
            {
                // sending message to all other connected user to notify them that the user is now disconnected
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}