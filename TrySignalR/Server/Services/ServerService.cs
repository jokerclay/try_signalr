using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using TrySignalR.Server.Hubs;

namespace TrySignalR.Server.Services;

public class ServerService :BackgroundService
{
    private readonly TimeSpan Period  = TimeSpan.FromSeconds(0.01);
    
    
    private readonly ILogger<ServerService> _logger;
    private readonly IHubContext<ReveiceMessageHub, IReveiceMessageClient> _serverContext;
    

    public ServerService(IHubContext<ReveiceMessageHub, IReveiceMessageClient> serverContext, ILogger<ServerService> logger)
    {
        _serverContext = serverContext;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string message = "test";
        
        // sent message to the client  
        using var timer = new PeriodicTimer(Period);

        using (Socket clientSocket =
               new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            
                IPAddress serverIP = IPAddress.Parse("127.0.0.1"); // Server IP address
                IPEndPoint serverEndPoint = new IPEndPoint(serverIP, 12345); // Server port
                clientSocket.Connect(serverEndPoint);
                
                
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Receive and display messages
                byte[] buffer = new byte[1024];
                int bytesReceived = clientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                // Console.WriteLine("Received: " + message);
            
                await _serverContext.Clients.All.ReceiveServerMessages($"服务器消息 : {message}");
            }
                
        }
        
        
    }
    
#if false
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                // Set up the client socket
                using (Socket clientSocket =
                       new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    // Connect to the server
                    IPAddress serverIP = IPAddress.Parse("127.0.0.1"); // Server IP address
                    IPEndPoint serverEndPoint = new IPEndPoint(serverIP, 12345); // Server port
                    clientSocket.Connect(serverEndPoint);

                    bool flag = true;
                    while (flag)
                    {
                        // Receive and display messages
                        byte[] buffer = new byte[1024];
                        int bytesReceived = clientSocket.Receive(buffer);
                        message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                        // Console.WriteLine("Received: " + message);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

         }
        
#endif
    
}