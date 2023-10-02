using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ln_backgroundServices {
public class WebSocketBackgroundService : IHostedService
{
    private readonly IHubContext<WebSocketHub> _hubContext;
    private List<WebSocket> _clients = new List<WebSocket>();
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
   public WebSocketBackgroundService(IHubContext<WebSocketHub> hubContext)
    {
        _hubContext = hubContext;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            // Start the WebSocket server logic here
            StartWebSocketServer();
            return Task.CompletedTask;
        }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        // Stop the WebSocket server logic here
        StopWebSocketServer();
        return Task.CompletedTask;
    }
    public Task StartWebSocketServer()
    {
          while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            // Handle WebSocket connections and messages via SignalR hub
            Task.Delay(1000); // Adjust delay as needed
        }
        // var listener = new HttpListener();
        // listener.Prefixes.Add("http://0.0.0.0:8080/"); // Adjust the URL as needed

        // listener.Start();

        // while (!_cancellationTokenSource.Token.IsCancellationRequested)
        // {
        //     var context = await listener.GetContextAsync();
        //     if (context.Request.IsWebSocketRequest)
        //     {
        //         var webSocketContext = await context.AcceptWebSocketAsync(null);
        //         var webSocket = webSocketContext.WebSocket;
        //         _clients.Add(webSocket);
        //         await HandleWebSocketConnection(webSocket);
        //     }
        //     else
        //     {
        //         context.Response.StatusCode = 400; // Bad Request
        //         context.Response.Close();
        //     }
        // }
        return Task.CompletedTask;
    }

    public void StopWebSocketServer()
    {
         _cancellationTokenSource.Cancel();
        foreach (var client in _clients)
        {
            client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down", CancellationToken.None);
        }
     
    }

    public async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // Handle the incoming message, e.g., parse and respond to instructions
                await ProcessMessage(message, webSocket);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                _clients.Remove(webSocket);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }
    }

    private async Task ProcessMessage(string message, WebSocket senderWebSocket)
    {
        // Your message processing logic here
        // You can send instructions or responses back to the senderWebSocket
    }
}

}



public class WebSocketHub : Hub
{
    // Add hub methods as needed for message handling
}