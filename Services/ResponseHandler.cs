using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Services
{
    public class ResponseHandler
    {
        private StreamHandler streamHandler;
        public ResponseHandler(StreamHandler streamHandler)
        {
            this.streamHandler = streamHandler;
        }

        internal void SendEntityToClient(string simId, Entity entity, EntityState entityState)
        {
            var entityeResponse = BuildReturnObject(entity, entityState);
            this.streamHandler.SendMessageToId(simId, entityeResponse, messageType.mapMovement);
        }

        internal object BuildReturnObject(Entity entity, EntityState entityState)
        {
            return new
            {
                entity = entity,
                state = entityState
            };
        }

    }
}
