# SignalR

## Backend:

1. In `server`  project, add services in the `program.cs` file
    ```csharp
    // **********************************
    // SignalR
    // on server side, SignalR is build in
    // it doesn't require install any packages
    // **********************************
    builder.Services.AddSignalR();
    ```
2. Define your Hubs, that the **client** will connect to 
      - it will implement the `Hub` base class
      - which you can use messaging between your server(your hub) and your client
      - `Hub` base class isn't strongly  type
      - we can define a client interface that will contain the methods that we can call on the client side(this case our blazor client)
         ```csharp
               // then we can only call the methods exposed on  the  `INotificationsHub`
               public class NotificationsHub:Hub<INotificationsHub>
               {


               }

               public interface INotificationsHub
               {
               // the method the that sent the message to the client
               Task ReceiveNotification(string message);
               }
        ```
3. Sent a message to the client when the client is connected to the hub
     ```csharp
        // then we can only call the methods exposed on  the  `INotificationsHub`
        public class NotificationsHub:Hub<INotificationsHub>
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
    ```
   
4. Config our hub with web api in `program.cs`
    ```csharp
   // config hub
   // your hub class and route
   app.MapHub<NotificationsHub>("/notifications");
    ```

5. write a service that sent a message to the client every second in `/Services/ServerTimeNotifier.cs`

    ```csharp

   public class ServerTimeNotifier:BackgroundService
   {
   // represent for how often  sent the notification to the SignalR client
   // every 1 second
   private readonly TimeSpan Period  = TimeSpan.FromSeconds(1);
   // inject logger
   private readonly ILogger<ServerTimeNotifier> _logger;

    // our signalR service 
    // `NotificationsHub`    -> implementations
    // `INotificationClient` -> definitions
    // `_context` using this context
    private readonly IHubContext<NotificationsHub, INotificationClient> _context;
    
    
    public ServerTimeNotifier(ILogger<ServerTimeNotifier> logger, IHubContext<NotificationsHub, INotificationClient> context)
    {
        _logger = logger;
        _context = context;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
            // sent message to the client  
            using var timer = new PeriodicTimer(Period);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
            // logging
            var datetime = DateTime.Now;
            _logger.LogInformation("正在执行服务{Service} {Time}",nameof(ServerTimeNotifier), datetime );
            
            // use hub context to sent the message to the client(this case blazor application)
            // sent message to all connected client
            await _context.Clients.All.ReceiveNotification($"服务器时间 = {datetime}");
            }
        }
    }
    ```
6. register  `ServerTimeNotifier`  service 
   ```csharp
   #region SignalR
   // **********************************
   // SignalR
   // on server side, SignalR is build in 
   // it doesn't require install any packages
   // **********************************
   builder.Services.AddSignalR();

   // service
   builder.Services.AddHostedService<ServerTimeNotifier>();    // start it when the backend application start
   #endregion
   ```

7. Add CORS services,  handle CORS issues
   ```csharp
   #region  CORS
   builder.Services.AddCors();
   #endregion
   ```
8. define CORS  policy
   ```csharp
   #region  CORS policy
   // AllowAny Client connect  to our backend to avoid cors issues
   // In production, configure policy to match  specific client application
   app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
   #endregion
   ```

## Frontend(client:wasm)
1. Install nuget  package for connecting to SignalR
   `Microsoft.AspNetCore.SignalR.Client`







