using Microsoft.AspNetCore.SignalR;
using TrySignalR.Server.Hubs;

namespace TrySignalR.Server.Services;

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