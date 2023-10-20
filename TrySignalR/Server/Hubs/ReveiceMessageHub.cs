using Microsoft.AspNetCore.SignalR;

namespace TrySignalR.Server.Hubs;

public class ReveiceMessageHub : Hub<IReveiceMessageClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Client(Context.ConnectionId).ReceiveServerMessages("您好！您已成功连接到服务器!");
        await base.OnConnectedAsync();
    }
}

public interface IReveiceMessageClient
{
    // the method the that sent the message to the client
    Task ReceiveServerMessages(string message);
}
