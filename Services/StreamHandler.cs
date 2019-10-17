using Newtonsoft.Json;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Server.Services
{
    public class StreamHandler
    {

        private Dictionary<string, WebSocket> webSocketsCollection;
        private Dictionary<string, DateTime> keepAliveSockets;

        public StreamHandler()
        {

            webSocketsCollection = new Dictionary<string, WebSocket>();
            keepAliveSockets = new Dictionary<string, DateTime>();
        }

        internal bool NewConnectionRecieved(WebSocket webSocket, string id)
        {
            if (!webSocketsCollection.ContainsKey(id))
            {
                webSocketsCollection.Add(id, webSocket);
                return true;
            }
            return false;
        }

        internal async void SendMessageToId(string socketId, object message, messageType type)
        {
            if (webSocketsCollection.ContainsKey(socketId))
            {
                var socket = webSocketsCollection[socketId];
                ReturnStream rs = new ReturnStream()
                {
                    messageStatus = resultStatus.success,
                    messageType = type,
                    data = message
                };
                var bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(rs));
                var arraySegment = new ArraySegment<byte>(bytes);
                await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else // todo it not found ??
            { }


        }

        internal void KeepAlive(string id)
        {
            if (keepAliveSockets.ContainsKey(id))
            {
                try
                {

                    keepAliveSockets[id] = DateTime.Now;
                }
                catch (Exception ex)
                {
                }
            }
            else
                keepAliveSockets.Add(id, DateTime.Now);
        }

        internal Dictionary<string, DateTime> getAllStreams()
        {
            return this.keepAliveSockets;
        }

        internal void CloseConnection(string key)
        {
            if (this.webSocketsCollection.ContainsKey(key))
            {
                var webSocket = this.webSocketsCollection[key];
                webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, webSocket.CloseStatusDescription, CancellationToken.None);

                this.webSocketsCollection.Remove(key);
            }
        }
    }
}
