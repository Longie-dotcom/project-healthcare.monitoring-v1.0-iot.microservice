using Microsoft.AspNetCore.SignalR;

namespace Signal
{
    public class SignalHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("ReceiveGroupMessage",
                $"Joined group: {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("ReceiveGroupMessage",
                $"Left group: {groupName}");
        }
    }
}
