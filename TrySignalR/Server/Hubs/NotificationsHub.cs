using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TrySignalR.Server.Hubs;


// use Authorize to control who can access this hub
[Authorize]

// then we can only call the methods exposed on  the  `INotificationsHub`
public class NotificationsHub:Hub<INotificationClient>
{
    // invoke `OnConnectedAsync`, you can thinking it in the way of `OnInitializedAsync` in blazor
    // as soon as a client connected to our hub, we can do the things we want to do
    // most common case is  say someone(client is joined)
    public override async Task OnConnectedAsync()
    {
        // we can use `Clients` to access all the client 
        // we can reference the `Client` to talk to a specific client by calling a `Client` method
        // it require  a `connectionId`, we can get it from the hub caller context,
        // it contain the `ConnectionId` property which is the client connected
        // so, when a new client connected to  the hub, it will sent a message to the client
        await Clients.Client(Context.ConnectionId).ReceiveNotification($"{Context.User?.Identity?.Name}, 您好！您已成功连接到服务器! ");
        await base.OnConnectedAsync();
    }
}

public interface INotificationClient
{
    // the method the that sent the message to the client
    Task ReceiveNotification(string message);
}




