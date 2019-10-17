using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : Controller
    {
        private StreamHandler streamHandler;

        public StreamController(StreamHandler streamHandler)
        {
            this.streamHandler = streamHandler;
        }

        [Route("NewConnection/{id}")]
        [HttpGet]
        public async Task NewConnection(string id)
        {
            var buffer = new byte[1024 * 4];

            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {

                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                //WebSocketConnection webSocketConnection = new WebSocketConnection(webSocket);

                if (this.streamHandler.NewConnectionRecieved(webSocket, id))
                {
                    //triggerHandler.PrepareNewSimulation(id);
                }
                else
                {
                    //todo - think here... same id came twice
                }
                var message = new { data = "connection success" };
                this.streamHandler.SendMessageToId(id, message, Models.messageType.newConnection);
                //todo - think of a better way...
                //this keeps the connection open
                while (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.Connecting)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        System.Console.WriteLine("closing connection");
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, webSocket.CloseStatusDescription, CancellationToken.None);
                        System.Console.WriteLine("connection closed");
                    }
                    //this.streamHandler.KeepAlive(id);
                }

            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

    }
}